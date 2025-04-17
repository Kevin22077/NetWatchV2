﻿using Microsoft.EntityFrameworkCore;
using NetWatchV2.Models;

namespace NetWatchV2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Contenido> Contenidos { get; set; }
    }
}
