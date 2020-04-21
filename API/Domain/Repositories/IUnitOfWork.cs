﻿namespace API.Domain.Repositories
{
    public interface IUnitOfWork
    {
        System.Threading.Tasks.Task CompleteAsync();
    }
}
