using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Infraestructura_API.Identidad;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura_API.Persistencia
{
    public class AppDbContext : IdentityDbContext<AppIdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Libro> Libros { get; set; }

        public DbSet<Autor> Autores { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Reserva> Reservas { get; set; }

        public DbSet<LibroReserva> LibrosReservas { get; set; }

        public DbSet<AutorLibro> AutoresLibros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Muchos a Muchos: Book <-> Author
            modelBuilder.Entity<AutorLibro>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<AutorLibro>()
                .HasOne(ba => ba.Libro)
                .WithMany(b => b.AutoresLibro)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<AutorLibro>()
                .HasOne(ba => ba.Autor)
                .WithMany(a => a.LibrosAutor)
                .HasForeignKey(ba => ba.AuthorId);

            // Relación Muchos a Muchos: Book <-> Reserve
            modelBuilder.Entity<LibroReserva>()
                .HasKey(br => new { br.BookId, br.ReserveId });

            modelBuilder.Entity<LibroReserva>()
                .HasOne(br => br.Libro)
                .WithMany(b => b.LibroReservas)
                .HasForeignKey(br => br.BookId);

            modelBuilder.Entity<LibroReserva>()
                .HasOne(br => br.Reserva)
                .WithMany(r => r.ReservaLibros)
                .HasForeignKey(br => br.ReserveId);

            //Relación entre usuario y reserva
            modelBuilder.Entity<Reserva>()
                .HasOne<AppIdentityUser>()
                .WithMany() // Un usuario puede tener muchas reservas
                .HasForeignKey(r => r.UserId)
                .IsRequired(); // Una reserva no puede existir sin un usuario

            // Configuración de Muchos a Muchos: Libro <-> Categoria
            modelBuilder.Entity<LibroCategoria>()
                .HasKey(lc => new { lc.LibroId, lc.CategoriaId });

            modelBuilder.Entity<LibroCategoria>()
                .HasOne(lc => lc.Libro)
                .WithMany(l => l.LibroCategorias)
                .HasForeignKey(lc => lc.LibroId);

            modelBuilder.Entity<LibroCategoria>()
                .HasOne(lc => lc.Categoria)
                .WithMany(c => c.CategoriaLibros)
                .HasForeignKey(lc => lc.CategoriaId);
        }
    }
}
