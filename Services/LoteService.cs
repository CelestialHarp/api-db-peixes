using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Enums;
using API_DB_PESCES_em_C__bonitona.Models;
using Microsoft.EntityFrameworkCore;

namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class LoteService
    {
        private readonly DbPescesContext _context;

        public LoteService(DbPescesContext context)
        {
            _context = context;
        }

        public async Task<LoteResponseDTO> CriarLoteAsync(CreateLoteDTO dto)
        {
            var novoLote = new Lote
            { 
                Descricao = dto.Descricao,
                Status = (int)StatusLote.Aberto,
                QuantidadePeixes = 0,
                PrecoLote = 0,
                //Status = StatusLote.Aberto
            };

            _context.Lotes.Add(novoLote);
            await _context.SaveChangesAsync();

            return MapToResponse(novoLote, 0);
        }
        
        public async Task<LoteResponseDTO> ObterDetalhesLoteAsync(int id)
        {
            
            var lote = await _context.Lotes
                .Include(l => l.Pesces)
                .ThenInclude(p => p.Especie)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lote == null) throw new Exception("Lote não existe.");

            // 2. O CÁLCULO DA SUGESTÃO (A "Lógica Normal" dentro do Service)
            decimal somaSugestao = 0;

            // 
            // ATENÇÃO: Fazer query dentro de foreach é ruim (N+1 problem), mas como um lote não tem e provavelmente nunca vai ter milhares de peixes, usar essa estratégia não tem tanto problema.
            // O ideal seria carregar todos os preços relevantes em memória antes.
            
            foreach(var peixe in lote.Pesces)
            {

                var precoMatriz = await _context.Precos
                    .Where(p => p.EspecieId == peixe.EspecieId&&
                    p.EstadoSaudeId == peixe.EstadoSaudeId&&
                    p.EstadoDesenvolvimentoId == peixe.EstadoDesenvolvimentoId)
                    .Select(p => p.Valor)
                    .FirstOrDefaultAsync();
                    
                somaSugestao += precoMatriz ?? 0;
            }

            return MapToResponse(lote, somaSugestao);
        }

        //Código obsoleto. Vou estudar se posso removê-lo por completo, isto é, sem que haja necessidade de um código substituto para o cumprimento da regra de negócio relacionada.
        public async Task FinalizarVendaAsync(int loteId, decimal precoFinal)
        {
            var lote = await _context.Lotes.FindAsync(loteId);

            if (lote == null) throw new Exception("Lote não encontrado.");

            if (lote.Status == (int)StatusLote.Vendido)
            {
                throw new InvalidOperationException("Este lote já foi vendido e não pode ser alterado.");
            }

            lote.PrecoLote = precoFinal;
            lote.Status = (int)StatusLote.Vendido;

            await _context.SaveChangesAsync();
        }

        private LoteResponseDTO MapToResponse(Lote lote, decimal somaSugestao)
        {
            // Aqui você transformaria a lista de Models de Peixe em DTOs
            var peixesDto = lote.Pesces.Select(p => new PeixeResponseDTO(
                //falta ainda adicionar os outros campos do DTO (There is no argument given that corresponds to the required parameter 'EstadoDeDesenvolvimentoId' of 'PeixeResponseDTO.PeixeResponseDTO(int, int?, int, int, int, string, string, decimal)'), só que provavelmente vai dar o mesmo erro que dá no PesceService.cs
                p.Id,
                p.LoteId,
                p.EspecieId,
                p.EstadoSaudeId,
                p.EstadoDesenvolvimentoId,
                p.Especie?.NomeVulgar ?? "N/A", // Requer .Include no nível da espécie para obter o nome
                p.Sexo,
                0                               // O preço seria calculado aqui se necessário
            )).ToList();
            return new LoteResponseDTO(
                lote.Id,
                lote.Descricao,
                lote.Pesces.Count(),
                new List<PeixeResponseDTO>(), // (Simplificado para o exemplo)
                lote.PrecoLote ?? 0,          // Preço se já estiver fechado
                somaSugestao                  // <--valor calculado
            );
        }
    }
}