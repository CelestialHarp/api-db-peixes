using Microsoft.EntityFrameworkCore;

namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class DbPescesContext : DbContext
{
    public DbPescesContext()
    {
    }

    public DbPescesContext(DbContextOptions<DbPescesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comportamento> Comportamentos { get; set; }

    public virtual DbSet<GuildaTrofica> GuildasTroficas { get; set; }

    public virtual DbSet<Especie> Especies { get; set; }

    public virtual DbSet<EstadoDesenvolvimento> EstadosDesenvolvimentos { get; set; }

    public virtual DbSet<EstadoSaude> EstadosSaudes { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

    public virtual DbSet<Pesce> Pesces { get; set; }

    public virtual DbSet<Preco> Precos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Carrinho> Carrinhos { get; set; }

    public virtual DbSet<ItemCarrinho> ItensCarrinho { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<ItemPedido> ItensPedido { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;port=5432;Database=DB_PESCES;username=postgres;password=");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comportamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comportamentos_pkey");

            entity.ToTable("comportamentos");

            entity.HasIndex(e => e.Descricao, "comportamentos_descricao_key").IsUnique();

            entity.HasIndex(e => e.Nome, "comportamentos_nome_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(250)
                .HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Especie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("especies_pkey");

            entity.ToTable("especies");

            entity.HasIndex(e => e.ImagemUrl, "especies_imagem_url_key").IsUnique();

            entity.HasIndex(e => e.NomeVulgar, "especies_nome_vulgar_key").IsUnique();

            entity.HasIndex(e => e.Taxon, "especies_taxon_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comentario).HasColumnName("comentario");
            entity.Property(e => e.ComportamentoId).HasColumnName("comportamento_id");
            entity.Property(e => e.ImagemUrl)
                .HasMaxLength(255)
                .HasColumnName("imagem_url");
            entity.Property(e => e.Linhagem)
                .HasMaxLength(250)
                .HasColumnName("linhagem");
            entity.Property(e => e.NomeVulgar)
                .HasMaxLength(250)
                .HasColumnName("nome_vulgar");
            entity.Property(e => e.Subespecie)
                .HasMaxLength(250)
                .HasColumnName("subespecie");
            entity.Property(e => e.Taxon)
                .HasMaxLength(250)
                .HasColumnName("taxon");

            entity.HasOne(d => d.Comportamento).WithMany(p => p.Especies)
                .HasForeignKey(d => d.ComportamentoId)
                .HasConstraintName("fk_comportamento");
            entity.HasOne(d => d.GuildaTrofica).WithMany()
                .HasForeignKey(d => d.GuildaTroficaId)
                .HasConstraintName("fk_guilda_trofica");
        });

        modelBuilder.Entity<EstadoDesenvolvimento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estados_desenvolvimento_pkey");

            entity.ToTable("estados_desenvolvimento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(250)
                .HasColumnName("descricao");
        });

        modelBuilder.Entity<EstadoSaude>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estados_saude_pkey");

            entity.ToTable("estados_saude");

            entity.HasIndex(e => e.Descricao, "estados_saude_descricao_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(250)
                .HasColumnName("descricao");
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lotes_pkey");

            entity.ToTable("lotes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.PrecoLote)
                .HasDefaultValue(0m)
                .HasColumnName("preco_lote");
            entity.Property(e => e.QuantidadePeixes)
                .HasDefaultValue(0)
                .HasColumnName("quantidade_peixes");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Pesce>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pesces_pkey");

            entity.ToTable("pesces");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento");
            entity.Property(e => e.EspecieId).HasColumnName("especie_id");
            entity.Property(e => e.EstadoDesenvolvimentoId).HasColumnName("estado_desenvolvimento_id");
            entity.Property(e => e.EstadoSaudeId).HasColumnName("estado_saude_id");
            entity.Property(e => e.LoteId).HasColumnName("lote_id");
            entity.Property(e => e.Sexo)
                .HasMaxLength(10)
                .HasColumnName("sexo");

            entity.HasOne(d => d.Especie).WithMany(p => p.Pesces)
                .HasForeignKey(d => d.EspecieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_especie_peixe");

            entity.HasOne(d => d.EstadoDesenvolvimento).WithMany(p => p.Pesces)
                .HasForeignKey(d => d.EstadoDesenvolvimentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_estado_desenv_peixe");

            entity.HasOne(d => d.EstadoSaude).WithMany(p => p.Pesces)
                .HasForeignKey(d => d.EstadoSaudeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_estado_saude_peixe");

            entity.HasOne(d => d.Lote).WithMany(p => p.Pesces)
                .HasForeignKey(d => d.LoteId)
                .HasConstraintName("fk_lote_peixe");
        });

        modelBuilder.Entity<Preco>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("precos_pkey");

            entity.ToTable("precos");

            entity.HasIndex(e => new { e.EspecieId, e.EstadoSaudeId, e.EstadoDesenvolvimentoId }, "precos_especie_id_estado_saude_id_estado_desenvolvimento_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EspecieId).HasColumnName("especie_id");
            entity.Property(e => e.EstadoDesenvolvimentoId).HasColumnName("estado_desenvolvimento_id");
            entity.Property(e => e.EstadoSaudeId).HasColumnName("estado_saude_id");
            entity.Property(e => e.Valor)
                .HasPrecision(6, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.Especie).WithMany(p => p.Precos)
                .HasForeignKey(d => d.EspecieId)
                .HasConstraintName("fk_especie_preco");

            entity.HasOne(d => d.EstadoDesenvolvimento).WithMany(p => p.Precos)
                .HasForeignKey(d => d.EstadoDesenvolvimentoId)
                .HasConstraintName("fk_estado_desenv_preco");

            entity.HasOne(d => d.EstadoSaude).WithMany(p => p.Precos)
                .HasForeignKey(d => d.EstadoSaudeId)
                .HasConstraintName("fk_estado_saude_preco");
        });

        //asasa
        modelBuilder.Entity<Carrinho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carrinhos_pkey");
            
            // Garante que cada usuário só tem 1 carrinho ativo
            entity.HasIndex(e => e.UsuarioId, "carrinhos_usuario_id_key").IsUnique();

            entity.HasOne(d => d.Usuario).WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("fk_usuario_carrinho");
        });

        modelBuilder.Entity<ItemCarrinho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itens_carrinho_pkey");

            // A MÁGICA: Garante que o peixe não pode estar em dois carrinhos ao mesmo tempo
            entity.HasIndex(e => e.PesceId, "itens_carrinho_pesce_id_key").IsUnique();

            entity.HasOne(d => d.Carrinho).WithMany(p => p.Itens)
                .HasForeignKey(d => d.CarrinhoId)
                .HasConstraintName("fk_carrinho_item");

            entity.HasOne(d => d.Pesce).WithMany()
                .HasForeignKey(d => d.PesceId)
                .HasConstraintName("fk_pesce_item");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pedidos_pkey");

            entity.HasOne(d => d.Usuario).WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("fk_usuario_pedido");
        });

        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itens_pedido_pkey");

            // A MÁGICA: Garante que o peixe só pode ser vendido uma única vez na história
            entity.HasIndex(e => e.PesceId, "itens_pedido_pesce_id_key").IsUnique();

            entity.HasOne(d => d.Pedido).WithMany(p => p.Itens)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("fk_pedido_item");

            entity.HasOne(d => d.Pesce).WithMany()
                .HasForeignKey(d => d.PesceId)
                .HasConstraintName("fk_pesce_pedido");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
