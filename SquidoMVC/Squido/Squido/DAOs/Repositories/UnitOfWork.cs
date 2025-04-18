﻿using System.ComponentModel.DataAnnotations;
using Squido.DAOs.Interfaces;
using Squido.Models;
using Squido.Models.Entities;

namespace Squido.DAOs.Repositories;

public class UnitOfWork  : IUnitOfWork
{
    private readonly SquidoDbContext _context;
    private GenericRepository<Book>? _bookRepository;
    private GenericRepository<ImageBook>? _imageBookRepository;
    private GenericRepository<Role>? _roleRepository;
    private GenericRepository<User>? _userRepository;
    private GenericRepository<Category>? _categoryRepository;
    private GenericRepository<Order>? _orderRepository;
    private GenericRepository<OrderItem> _orderItemRepository;
    private GenericRepository<Rating>? _ratingRepository;

    public UnitOfWork(SquidoDbContext context)
    {
        _context = context;
      
    }

    public IGenericRepository<Book> BookRepository
    {
        get
        {
            return _bookRepository ??= new GenericRepository<Book>(_context);
        }
    }

    public IGenericRepository<User> UserRepository
    {
        get
        {
            return _userRepository ??= new GenericRepository<User>(_context);
        }
    }

    public IGenericRepository<Category> CategoryRepository
    {
        get
        {
            return _categoryRepository ??= new GenericRepository<Category>(_context);
        }
    }

    public IGenericRepository<Order> OrderRepository
    {
        get
        {
            return _orderRepository ??= new GenericRepository<Order>(_context);
        }
    }

    public IGenericRepository<OrderItem> OrderItemRepository
    {
        get
        {
            return _orderItemRepository ??= new GenericRepository<OrderItem>(_context);
        }
    }

    public IGenericRepository<Rating> RatingRepository
    {
        get
        {
            return _ratingRepository ??= new GenericRepository<Rating>(_context);
        }
    }

    public IGenericRepository<Role> RoleRepository
    {
        get
        {
            return _roleRepository ??= new GenericRepository<Role>(_context);
        }
    }

    public IGenericRepository<ImageBook> ImageBookRepository
    {
        get
        {
            return _imageBookRepository ??= new GenericRepository<ImageBook>(_context);
        }
    }

    public void Save()
    {
        var validationErrors = _context.ChangeTracker.Entries<IValidatableObject>()
            .SelectMany(e => e.Entity.Validate(null))
            .Where(e => e != ValidationResult.Success)
            .ToArray();
        if (validationErrors.Any())
        {
            var exceptionMessage = string.Join(Environment.NewLine,
                validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
            throw new Exception(exceptionMessage);
        }
        _context.SaveChanges();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}