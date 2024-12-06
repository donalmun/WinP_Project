// WinP_Project\FoodApp\Service\DataAccess\IDao.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp.Service.DataAccess;

public interface IDao<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(int id);

    Task<bool> TestConnectionAsync();
}