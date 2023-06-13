using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using MongoDB.Driver;
using ZstdSharp;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        protected IMongoCollection<Product> products => _context.Products;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateProduct(Product product)
        {
            await products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            //defines a filter to delete specific data
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            //gets a result data after delete operation
            DeleteResult deleteResult = await products.DeleteOneAsync(filter);

            //returns true if delete operation is successfull
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
            
            ReplaceOneResult updateResult = await products.ReplaceOneAsync(
                filter: filter,
                replacement: product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }


        #region ReadMethods
        public async Task<Product> GetProduct(string id)
        {
            return await products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
            return await products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            return await products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var productsResult = await products.Find(p => true).ToListAsync();
            return productsResult;
        }
        #endregion
    }
}
