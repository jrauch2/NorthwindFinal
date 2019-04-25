using System.Linq;

namespace Northwind.Models
{
    public interface INorthwindRepository
    {
        IQueryable<Category> Categories { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Discount> Discounts { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Employees> Employees { get; }

        void AddCustomer(Customer customer);
        void AddEmployee(Employees employee);
        void EditCustomer(Customer customer);
        void EditEmployee(Employees employee);
    }
}
