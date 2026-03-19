using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Models;
using Microsoft.EntityFrameworkCore;

namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class PrecoService
    {
        private readonly DbPescesContext _context;
        
        public PrecoService(DbPescesContext context)
        {
            _context = context;
        }
        
        public void DefinirPreco(CreatePrecoDTO dto)
        {
            var precoExistente = _context.Precos
            .FirstOrDefault(p => 
                p.EspecieId == dto.EspecieId &&
                p.EstadoSaudeId == dto.EstadoDeSaudeId &&
                p.EstadoDesenvolvimentoId == dto.EstadoDeDesenvolvimentoID
            );

        if (precoExistente != null)
        {
            precoExistente.Valor = dto.Valor;
        }
        else
        {
            var novoPreco = new Preco
            {
                EspecieId = dto.EspecieId,
                EstadoSaudeId = dto.EstadoDeSaudeId,
                EstadoDesenvolvimentoId = dto.EstadoDeDesenvolvimentoID,
                Valor = dto.Valor
            };
            _context.Precos.Add(novoPreco);
        }
        //Acho que talvez devesse ter algum tipo de tratamento de erro específico aqui. Depois vou pesquisar sobre.

        // Salva as mudanças (seja inserção ou atualização)
        _context.SaveChanges();

        }
        public List<ResponsePrecoDTO> ListarTodos()
        {
            return _context.Precos
                .Include(p => p.Especie)
                .Include(p => p.EstadoSaude)
                .Include(p => p.EstadoDesenvolvimento)
                .Select(p => new ResponsePrecoDTO(
                    p.Id,
                    p.Valor ?? 0,
                    p.Especie.NomeVulgar,
                    p.EstadoSaude.Descricao,
                    p.EstadoDesenvolvimento.Descricao
                ))
                .ToList();
        }
    }
}