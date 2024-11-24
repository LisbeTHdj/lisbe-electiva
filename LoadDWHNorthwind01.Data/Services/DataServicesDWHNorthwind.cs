using LoadDWHNorthwind01.Data.Context;
using LoadDWHNorthwind01.Data.Entities.DWHNorthwind;
using LoadDWHNorthwind01.Data.Entities.DWHNothwind;
using LoadDWHNorthwind01.Data.Interfaces;
using LoadDWHNorthwind01.Data.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadDWHNorthwind01.Data.Services
{
    public class DataServicesDWHNorthwind : IDataServicesDWHNorthwind
    {
        private readonly NorthwindContext _northwindContext;
        private readonly DbNorthwindContext _dbNorthwindContext;
        private readonly ILogger<DataServicesDWHNorthwind> _logger;

        // Constructor with logging injection
        public DataServicesDWHNorthwind(NorthwindContext northwindContext,
            DbNorthwindContext dbNorthwindContext,
            ILogger<DataServicesDWHNorthwind> logger)
        {
            _northwindContext = northwindContext;
            _dbNorthwindContext = dbNorthwindContext;
            _logger = logger;
        }

        public async Task<OperationResult> LoadDWH()
        {
            OperationResult result = new OperationResult();
            try
            {
                _logger.LogInformation("Starting DWH load...");

                
                 //await LoadDimEmployees();
                 await LoadDimProducts();
                // await LoadDimCustomers();
                //await LoadDimShippers();

                _logger.LogInformation("DWH Load Complete!");

                result.Success = true;
                result.Message = "DWH Load Complete!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading DWH Northwind");
                result.Success = false;
                result.Message = $"Error loading DWH Northwind: {ex.Message}";
            }
            return result;
        }

        private async Task<OperationResult> LoadDimEmployees()
        {
            OperationResult result = new OperationResult();
            try
            {
                _logger.LogInformation("Loading DimEmployees...");

                // Verificar si ya existen registros en DimEmployees para evitar duplicados
                var existingEmployeeKeys = await _dbNorthwindContext.DimEmployees
                    .Select(de => de.EmployeeKey) // Solo seleccionar las claves primarias
                    .ToListAsync();

                // Cargar los empleados desde Northwind, pero solo si no existen en DimEmployees y evitar nulos
                var employeesToLoad = await _northwindContext.Employees
                    .AsNoTracking()
                    .Where(employee => !existingEmployeeKeys.Contains(employee.EmployeeId) &&
                                       !string.IsNullOrEmpty(employee.FirstName) &&
                                       !string.IsNullOrEmpty(employee.LastName)) // Evitar nulos o vacíos
                    .Select(employee => new DimEmployees
                    {
                        // No asignar EmployeeKey explícitamente. Deja que SQL Server lo maneje como IDENTITY.
                        LastName = employee.LastName,
                        FirstName = employee.FirstName
                    }).ToListAsync();

                // Si no hay empleados nuevos para cargar, evitar continuar
                if (!employeesToLoad.Any())
                {
                    _logger.LogInformation("No new DimEmployees to load.");
                    result.Success = false;
                    result.Message = "No new DimEmployees to load.";
                    return result;
                }

                // Dividir la lista de empleados en bloques más pequeños para evitar problemas de tamaño
                var chunkSize = 100; // Tamaño del bloque de inserción
                var chunkedEmployees = employeesToLoad
                    .Select((employee, index) => new { Index = index, Employee = employee })
                    .GroupBy(x => x.Index / chunkSize)
                    .Select(group => group.Select(x => x.Employee).ToList())
                    .ToList();

                // Iniciar una transacción para asegurar la inserción
                using var transaction = await _dbNorthwindContext.Database.BeginTransactionAsync();

                try
                {
                    // Insertar los empleados en bloques
                    foreach (var chunk in chunkedEmployees)
                    {
                        await _dbNorthwindContext.DimEmployees.AddRangeAsync(chunk);
                        await _dbNorthwindContext.SaveChangesAsync();
                    }

                    // Confirmar transacción
                    await transaction.CommitAsync();

                    _logger.LogInformation("DimEmployees loaded successfully.");
                    result.Success = true;
                    result.Message = "DimEmployees loaded successfully.";
                }
                catch (Exception ex)
                {
                    // Revertir los cambios en caso de error
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error loading DimEmployees during transaction.");
                    result.Success = false;
                    result.Message = $"Error loading DimEmployees: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading DimEmployees");
                result.Success = false;
                result.Message = $"Error loading DimEmployees: {ex.Message}";
            }

            return result;
        }

        private async Task<OperationResult> LoadDimProducts()
        {
            OperationResult result = new OperationResult();

            try
            {
                _logger.LogInformation("Loading DimProducts...");

                // Verificar si ya existen registros en DimProducts para evitar duplicados
                var existingProductKeys = await _dbNorthwindContext.DimProducts
                    .Select(dp => dp.ProductKey) // Seleccionar solo las claves primarias
                    .ToListAsync();

                // Cargar productos desde Northwind que aún no existen en DimProducts
                var productsToLoad = await _northwindContext.Products
                    .AsNoTracking()
                    .Where(product => !existingProductKeys.Contains(product.ProductId)) // Evitar duplicados
                    .Join(_northwindContext.Categories.AsNoTracking(), // Hacer join con las categorías
                          product => product.Category,  // Usar Category en lugar de CategoryID
                          category => category.CategoryID,
                          (product, category) => new DimProducts
                          {
                              ProductKey = product.ProductId, // Clave primaria del producto
                              ProductID = product.SupplierId, // ID del proveedor (puede ser null)
                              Name = product.QuantityPerUnit,     // Utilizar otra propiedad, ya que ProductName no está definida
                              CategoryKey = category.CategoryID // Clave primaria de la categoría
                          })
                    .ToListAsync();

                // Si no hay productos nuevos para cargar, evitar continuar
                if (!productsToLoad.Any())
                {
                    _logger.LogInformation("No new DimProducts to load.");
                    result.Success = false;
                    result.Message = "No new DimProducts to load.";
                    return result;
                }

                // Insertar los productos en bloques pequeños para evitar problemas de memoria
                var chunkSize = 100; // Tamaño de cada bloque
                var chunkedProducts = productsToLoad
                    .Select((product, index) => new { Index = index, Product = product })
                    .GroupBy(x => x.Index / chunkSize)
                    .Select(group => group.Select(x => x.Product).ToList())
                    .ToList();

                // Usar una transacción para asegurar la consistencia
                using var transaction = await _dbNorthwindContext.Database.BeginTransactionAsync();

                try
                {
                    foreach (var chunk in chunkedProducts)
                    {
                        await _dbNorthwindContext.DimProducts.AddRangeAsync(chunk);
                        await _dbNorthwindContext.SaveChangesAsync();
                    }

                    // Confirmar transacción
                    await transaction.CommitAsync();

                    _logger.LogInformation("DimProducts loaded successfully.");
                    result.Success = true;
                    result.Message = "DimProducts loaded successfully.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); 
                    _logger.LogError(ex, "Error loading DimProducts during transaction.");
                    result.Success = false;
                    result.Message = $"Error loading DimProducts: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading DimProducts");
                result.Success = false;
                result.Message = $"Error loading DimProducts: {ex.Message}";
            }

            return result;
        }



        private async Task<OperationResult> LoadDimCustomers()
        {
            OperationResult result = new OperationResult();
            try
            {
                _logger.LogInformation("Loading DimCustomers...");

               
                var existingCustomers = await _dbNorthwindContext.DimCustomers.ToListAsync();

                if (existingCustomers.Any())
                {
                    _logger.LogInformation("DimCustomers already loaded.");
                    result.Success = false;
                    result.Message = "DimCustomers already loaded.";
                    return result;
                }

                
                var customers = _northwindContext.Customers.AsNoTracking()
                    .Select(customer => new DimCustomers
                    {
                        CustKey = customer.CustomerID, 
                        CustName = customer.CompanyName,
                        CustID = customer.CustomerID.ToString() 
                    }).ToList();

                // Agregar los datos a DimCustomers
                await _dbNorthwindContext.DimCustomers.AddRangeAsync(customers);
                await _dbNorthwindContext.SaveChangesAsync();

                _logger.LogInformation("DimCustomers loaded successfully.");
                result.Success = true;
                result.Message = "DimCustomers loaded successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading DimCustomers");
                result.Success = false;
                result.Message = $"Error loading DimCustomers: {ex.Message}";
            }

            return result;
        }

        private async Task<OperationResult> LoadDimShippers()
        {
            OperationResult result = new OperationResult();
            try
            {
                _logger.LogInformation("Loading DimShippers...");

                var existingShippers = await _dbNorthwindContext.DimShippers.ToListAsync();

                
                if (existingShippers.Any())
                {
                    _logger.LogInformation("DimShippers already loaded.");
                    result.Success = false;
                    result.Message = "DimShippers already loaded.";
                    return result;
                }

                // Cargar los datos desde Northwind
                var shippers = _northwindContext.Shippers.AsNoTracking()
                    .Select(shipper => new DimShippers
                    {
                        ShipViaKey = shipper.ShipperID,
                        Name = shipper.CompanyName
                    }).ToList();

                // Agregar los datos a DimShippers
                await _dbNorthwindContext.DimShippers.AddRangeAsync(shippers);
                await _dbNorthwindContext.SaveChangesAsync();

                _logger.LogInformation("DimShippers loaded successfully.");
                result.Success = true;
                result.Message = "DimShippers loaded successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading DimShippers");
                result.Success = false;
                result.Message = $"Error loading DimShippers: {ex.Message}";
            }

            return result;
        }
    }
}
