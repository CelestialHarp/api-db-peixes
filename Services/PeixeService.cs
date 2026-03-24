using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Enums;
using API_DB_PESCES_em_C__bonitona.Models;
using Microsoft.EntityFrameworkCore;

namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class PeixeService
    {
        private readonly DbPescesContext _context;
        public PeixeService(DbPescesContext context)
        {
            _context = context;
        }

        public async Task<PeixeResponseDTO> AdicionarPeixe(CreatePeixeDTO dto)
        {
            if (dto.LoteId.HasValue)
            {
                var lote = await _context.Lotes.FindAsync(dto.LoteId.Value);

                
                if (lote == null) throw new Exception("O lote informado não existe.");


                if (lote.Status == (int)StatusLote.Vendido)
                {
                    throw new InvalidOperationException("Não é possível adicionar peixes a um lote já vendido/fechado.");
                }
            }

            var TabelaPrecos = await _context.Precos.
            //SELECT * FROM Precos WHERE ocorra essa combinação (Especie = X AND Saude = Y AND Desenv = Z)
            // if not found then null
            FirstOrDefaultAsync(p => 
                p.EspecieId == dto.EspecieId &&
                p.EstadoSaudeId == dto.EstadoDeSaudeId &&
                p.EstadoDesenvolvimentoId == dto.EstadoDeDesenvolvimentoId
            );
            if (TabelaPrecos == null) throw new Exception("Preço não cadastrado para essa combinação!");
            var especie = _context.Especies.Find(dto.EspecieId);
            if (especie == null) throw new Exception("Espécie inválida!");


            var novoPeixe = new Peixe
            {
                EspecieId = dto.EspecieId,
                LoteId = dto.LoteId, // Pode ser null
                EstadoSaudeId = dto.EstadoDeSaudeId,
                EstadoDesenvolvimentoId = dto.EstadoDeDesenvolvimentoId,
                DataNascimento = DateOnly.FromDateTime(DateTime.Now),
                Sexo = dto.Sexo // Ou viria do DTO
            };


            _context.Peixes.Add(novoPeixe);


            if (dto.LoteId.HasValue)
            {
                var lote = _context.Lotes.Find(dto.LoteId.Value);
                if (lote != null) lote.QuantidadePeixes++;
            }

            //Comittando. Só pra me lembrar, a operação toda só acontece de verdade aqui. Tudo é armazenado na memória como "contexto" pra depois ser executado.
            _context.SaveChanges();

            decimal ValorFinal = TabelaPrecos.Valor ?? 0;

            return new PeixeResponseDTO(
                novoPeixe.Id,
                novoPeixe.LoteId,              // 2º: int? LoteId
                /*
                  Esses IDs dão Cannot convert from int to int.
                */

                novoPeixe.EspecieId,
                novoPeixe.EstadoSaudeId,
                novoPeixe.EstadoDesenvolvimentoId,
                especie.NomeVulgar,
                novoPeixe.Sexo,
                ValorFinal
            );

        }
        //Desisti de colocar "pesce" em tudo. O dank não me deixa pensar direito, então vai ficar só como um efeite no banco de dados e talvez no nome dos arquivos. Depois vou tentar consertar tudo.
        public async Task<List<PeixeResponseDTO>> PesquisarPeixeAsync(string? nomeEspecie, int? ID)
        {

            var query =
                _context.Peixes
                .Include(p => p.Especie) 
                .AsQueryable();

            if (ID.HasValue)
            {
                query = query.Where(p => p.Id == ID.Value);
            }
            else if (!string.IsNullOrWhiteSpace(nomeEspecie))
            {
                query = query.Where(p => p.Especie.NomeVulgar.ToLower().Contains(nomeEspecie.ToLower()));
            }

            var peixesEncontrados = await query.ToListAsync();
            var resultado = new List<PeixeResponseDTO>();

            foreach (var p in peixesEncontrados)
            {
                
                var preco = await _context.Precos
                    .Where(pr => 
                    pr.EspecieId == p.EspecieId && 
                    pr.EstadoSaudeId == p.EstadoSaudeId && 
                    pr.EstadoDesenvolvimentoId == p.EstadoDesenvolvimentoId)
                    .Select(pr => pr.Valor)
                    .FirstOrDefaultAsync();

                resultado.Add(new PeixeResponseDTO(
                    p.Id,
                    p.LoteId,
                    p.EspecieId,
                    p.EstadoSaudeId,
                    p.EstadoDesenvolvimentoId,
                    p.Especie.NomeVulgar,
                    p.Sexo,
                    preco ?? 0
                ));
            }

            return resultado;

        }
    }
}