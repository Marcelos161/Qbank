using Microsoft.EntityFrameworkCore;
using QBankApi.Models;

namespace QBankApi.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }
        public DbSet<CartaoCredito> CartoesCredito { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<Conta>().ToTable("Conta");
            modelBuilder.Entity<Servico>().ToTable("Servico");
            modelBuilder.Entity<Transacao>().ToTable("Transacao");
            modelBuilder.Entity<Emprestimo>().ToTable("Emprestimo");
            modelBuilder.Entity<CartaoCredito>().ToTable("CartaoCredito");
            modelBuilder.Entity<Pagamento>().ToTable("Pagamento");

            // Configuração do relacionamento entre Cliente e Conta
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Contas)
                .WithOne(c => c.Cliente)
                .HasForeignKey(c => c.ClienteID);

            // Configuração do relacionamento entre Cliente e Emprestimo
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Emprestimos)
                .WithOne(e => e.Cliente)
                .HasForeignKey(e => e.ClienteID);

            // Configuração do relacionamento entre Cliente e CartaoCredito
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.CartoesCredito)
                .WithOne(cc => cc.Cliente)
                .HasForeignKey(cc => cc.ClienteID);

            // Configuração do relacionamento entre Transacao e Conta (origem)
            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.ContaOrigem)
                .WithMany(c => c.TransacoesOrigem)
                .HasForeignKey(t => t.ContaOrigemID);

            // Configuração do relacionamento entre Transacao e Conta (destino)
            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.ContaDestino)
                .WithMany(c => c.TransacoesDestino)
                .HasForeignKey(t => t.ContaDestinoID);

            // Configuração do relacionamento entre Servico e Pagamento
            modelBuilder.Entity<Servico>()
                .HasMany(s => s.Pagamentos)
                .WithOne(p => p.Servico)
                .HasForeignKey(p => p.ServicoID); // Assumindo que Pagamento tem uma propriedade ServicoID
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
