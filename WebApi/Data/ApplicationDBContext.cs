
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Models;
using BlueSelfCheckout.WebApi.Models.Customers;
using BlueSelfCheckout.WebApi.Models.Products;
using BlueSelfCheckout.WebApi.Models.Admin;
using BlueSelfCheckout.WebApi.Models.Production;
using BlueSelfCheckout.WebApi.Models.Orders;

namespace BlueSelfCheckout.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext (DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Numeration>().ToTable("ONMN");



            modelBuilder.Entity<PointOfSale>().ToTable("OPOS");

            modelBuilder.Entity<ProductCategory>().ToTable("OITC");
            modelBuilder.Entity<ProductGroup>().ToTable("OITG");
            modelBuilder.Entity<Device>().ToTable("ODVC");
            modelBuilder.Entity<Image>().ToTable("OIMG");



            modelBuilder.Entity<Customer>().ToTable("OCTR");
            modelBuilder.Entity<CustomerGroup>().ToTable("OCTG");

            modelBuilder.Entity<ProductTree>().ToTable("OITT");
            modelBuilder.Entity<ProductTreeItem1>(entity =>
            {
                // Configure the composite primary key
                // The key is a combination of ItemCode and LineNumber
                entity.HasKey(e => new { e.ItemCode, e.LineNumber, e.ProductTreeItemCode });

                // Set the table name
                entity.ToTable("ITT1");

                // Configure the one-to-many relationship
                // A ProductTree (parent) has many Items1 (children)
                // The existing code for the relationship is still valid
                entity.HasOne<ProductTree>() // No need to specify the property on the parent side if you don't have a navigation property
                      .WithMany(p => p.Items1)
                      .HasForeignKey(c => c.ProductTreeItemCode)
                      .HasPrincipalKey(p => p.ItemCode)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Configuración de Product
            modelBuilder.Entity<Product>()
                .ToTable("OITM")
                .HasKey(p => p.ItemCode);

            modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductTree)
            .WithOne()
            .HasForeignKey<ProductTree>(pt => pt.ItemCode)
            .HasPrincipalKey<Product>(p => p.ItemCode);

            // Configuración de ProductMaterial
            modelBuilder.Entity<ProductMaterial>()
                .ToTable("ITM1")
                .HasKey(pm => new { pm.ProductItemCode, pm.ItemCode }); // Clave primaria compuesta

            modelBuilder.Entity<ProductMaterial>()
                .HasOne(pm => pm.Product)
                .WithMany(p => p.Material)
                .HasForeignKey(pm => pm.ProductItemCode)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<ProductMaterial>(ProductMaterial =>
            {
                ProductMaterial.Property(pm => pm.Quantity)
                    .HasPrecision(19, 6); // Ajusta la precisión y escala según tus necesidades
            }

                );

            // Configuración de ProductAccompaniment
            modelBuilder.Entity<ProductAccompaniment>()
                .ToTable("ITM2")
                .HasKey(pa => new { pa.ProductItemCode, pa.ItemCode }); // Clave primaria compuesta

            modelBuilder.Entity<ProductAccompaniment>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.Accompaniment)
                .HasForeignKey(pa => pa.ProductItemCode)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();


            modelBuilder.Entity<SalesTaxCodes>().ToTable("OSTC");
            modelBuilder.Entity<ShippingTypes>().ToTable("OSHP");

            modelBuilder.Entity<WorkOrder>().ToTable("OWOR");
            modelBuilder.Entity<WorkOrderItem>().ToTable("WOR1");

            modelBuilder.Entity<OrderLine>().HasKey(ol => new { ol.DocEntry, ol.LineId });

            modelBuilder.Entity<OrderLine>()
                .HasOne(dl => dl.Order) // Un OrderLine pertenece a un Order
                .WithMany(o => o.OrderLines) // Un Order tiene muchos OrderLine
                .HasForeignKey(dl => dl.DocEntry) // La clave foránea en OrderLine es DocEntry
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Order>().ToTable("ORDR");


            modelBuilder.Entity<CategoryAccompaniment>(entity =>
            {
                // Configure the composite primary key
                entity.HasKey(e => new { e.CategoryItemCode, e.LineNumber });

                // Map the entity to the ITC1 table
                entity.ToTable("ITC1");

                // Configure precision for decimal fields
                entity.Property(e => e.Discount)
                      .HasPrecision(19, 6);

                entity.Property(e => e.EnlargementDiscount)
                      .HasPrecision(19, 6);

                // Configure relationships
                entity.HasOne(c => c.Category)
                      .WithMany(p => p.Accompaniments) // Si quieres navegación bidireccional, agrega la propiedad en ProductCategory
                      .HasForeignKey(c => c.CategoryItemCode)
                      .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict para evitar cascadas circulares

                entity.HasOne(c => c.AccompanimentProduct)
                      .WithMany()
                      .HasForeignKey(c => c.AccompanimentItemCode)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.EnlargementProduct)
                      .WithMany()
                      .HasForeignKey(c => c.EnlargementItemCode)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public DbSet<CategoryAccompaniment> CategoryAccompaniments { get; set; } = default!;

        public DbSet<OrderLine> OrderLines { get; set; } = default!;

        public DbSet<PointOfSale> PointOfSale { get; set; } = default!;

        public DbSet<ProductGroup> ProductGroup { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategory { get; set; } = default!;
        public DbSet<Device> Device { get; set; } = default!;
        public DbSet<Image> Image { get; set; } = default!;
        public DbSet<Customer> Customer { get; set; } = default!;
        public DbSet<CustomerGroup> CustomerGroup { get; set; } = default!;
        public DbSet<ProductTree> ProductTree { get; set; } = default!;
        public DbSet<ProductTreeItem1> ProductTreeItem1 { get; set; } = default!;

        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<ProductMaterial> Material { get; set; } = default!;
        public DbSet<ProductAccompaniment> Accompaniment { get; set; } = default!;
        public DbSet<Numeration> Numeration { get; set; } = default!;
        public DbSet<SalesTaxCodes> SalesTaxCodes { get; set; } = default!;
        public DbSet<WorkOrder> WorkOrder { get; set; } = default!;
        public DbSet<WorkOrderItem> WorkOrderItem { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;


    }// fin de la clase

}// fin del namespace
