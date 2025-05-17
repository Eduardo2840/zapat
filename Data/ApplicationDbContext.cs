using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using zapat.Models;

namespace zapat.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Register> DbSetRegisters { get; set; }
    public DbSet<Producto> DbSetProducto { get; set; }
    public DbSet<DetalleOrden> DbSetDetalleOrden { get; set; }
    public DbSet<Orden> DbSetOrden { get; set; }
    public DbSet<Orden> DbSetAdmin { get; set; }
    public DbSet<Pago> DbSetPago { get; set; }
    public DbSet<PreOrden> DbSetPreOrden { get; set; }

}
