
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
            modelBuilder.Entity<ProductTreeItem1>().ToTable("ITT1");
            // Configuración de Product
            modelBuilder.Entity<Product>()
                .ToTable("OITM")
                .HasKey(p => p.ItemCode);

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

        }

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
