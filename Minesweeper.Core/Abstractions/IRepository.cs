﻿using Minesweeper.Core.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core.Abstractions
{
    //Это на будущее, если нужно будет хранить в базе данных, то будем использовать паттерн репозиторий для получения данных из бд итд
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking, Expression<Func<T, bool>> filter = null, string includes = null);
        Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate);

        Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken, string includes = null);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken);

        bool Delete(T entity);

        void Update(T entity);

        Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);

        void SaveChanges();
    }
}
