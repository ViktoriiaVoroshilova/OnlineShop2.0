﻿using DataAccess.EF.Models;
using DataAccess.EF.Repositories;

namespace DataAccess.EF;

public interface IUnitOfWork
{
    public IGenericRepository<Item> ItemRepository { get; }

    public IGenericRepository<Category> CategoryRepository { get; }

    public Task SaveAsync();
}