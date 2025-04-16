using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zapat.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdenPagoMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        
            
            // Tabla t_orden
            migrationBuilder.CreateTable(
            name: "t_order",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserID = table.Column<string>(type: "text", nullable: true),
                Total = table.Column<decimal>(type: "numeric", nullable: false),
                Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_t_order", x => x.id);
            });

            // Tabla t_pago (con relación 1 a 1 hacia t_orden)
            migrationBuilder.CreateTable(
            name: "t_pago",
            columns: table => new
            {
                id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                NombreTarjeta = table.Column<string>(type: "text", nullable: true),
                NumeroTarjeta = table.Column<string>(type: "text", nullable: true),
                MontoTotal = table.Column<decimal>(type: "numeric", nullable: false),
                Status = table.Column<string>(type: "text", nullable: true),
                UserName = table.Column<string>(type: "text", nullable: true),
                OrdenId = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_t_pago", x => x.id);
                table.ForeignKey(
                    name: "FK_t_pago_t_order_OrdenId",
                    column: x => x.OrdenId,
                    principalTable: "t_order",
                    principalColumn: "id");
            });
            // Tabla t_producto

            migrationBuilder.CreateTable(
            name: "t_producto",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "text", nullable: false),
                Price = table.Column<decimal>(type: "numeric", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                ImageURL = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_t_producto", x => x.Id);
            });

            // Tabla t_order_detail
            migrationBuilder.CreateTable(
            name: "t_order_detail",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ProductoId = table.Column<int>(type: "integer", nullable: true),
                Cantidad = table.Column<int>(type: "integer", nullable: false),
                Precio = table.Column<decimal>(type: "numeric", nullable: false),
                ordenId = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_t_order_detail", x => x.id);
                table.ForeignKey(
                    name: "FK_t_order_detail_t_order_ordenId",
                    column: x => x.ordenId,
                    principalTable: "t_order",
                    principalColumn: "id");
                table.ForeignKey(
                    name: "FK_t_order_detail_t_producto_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "t_producto",
                    principalColumn: "Id");
            });


            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_ordenId",
                table: "t_order_detail",
                column: "ordenId");

            migrationBuilder.CreateIndex(
                name: "IX_t_pago_OrdenId",
                table: "t_pago",
                column: "OrdenId");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            
            migrationBuilder.DropTable(
                name: "t_order_detail");

            migrationBuilder.DropTable(
                name: "t_pago");

            migrationBuilder.DropTable(
                name: "t_producto");

            migrationBuilder.DropTable(
                name: "t_order");
                
           
        }
    }
}
