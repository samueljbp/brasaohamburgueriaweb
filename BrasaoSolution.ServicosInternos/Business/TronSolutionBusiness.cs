using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrasaoSolution.Model;
using BrasaoSolution.TronSolutionData.Context;

namespace BrasaoSolution.ServicosInternos.Business
{
    public class TronSolutionBusiness
    {
        public ServiceResultViewModel GetItensFromTron()
        {
            ServiceResultViewModel result = new ServiceResultViewModel { Succeeded = true, Errors = new List<string>(), data = null };

            try
            {
                TronSolutionData.Context.TronSolutionConnection tron = new TronSolutionConnection();

                var query = from item in tron.ITEM_CARDAPIO
                            join emp in tron.ITEM_CARDAPIOXEMPRESA on item.CD_ITEMCARDAPIO equals emp.CD_ITEMCARDAPIO
                            where item.CD_CLASSEITEMCARDAPIO != null && emp.VALOR_VENDA != null && (item.PODE_TELE_ENTREGA != null && item.PODE_TELE_ENTREGA.Value == 1)
                            orderby item.CD_ITEMCARDAPIO
                            select new ItemCardapioViewModel { CodItemCardapio = item.CD_ITEMCARDAPIO, Ativo = (emp.ATIVO == null || emp.ATIVO.Value == 0 ? false : true), CodClasse = item.CD_CLASSEITEMCARDAPIO.Value, Nome = item.DS_ITEMCARDAPIO, Preco = (Double)emp.VALOR_VENDA.Value };

                result.data = query.ToList();

                tron.Dispose();
                tron = null;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public ServiceResultViewModel GetClassesFromTron()
        {
            ServiceResultViewModel result = new ServiceResultViewModel { Succeeded = true, Errors = new List<string>(), data = null };

            try
            {
                TronSolutionData.Context.TronSolutionConnection tron = new TronSolutionConnection();

                var query = from classe in tron.CLASSE_ITEMCARDAPIO
                            where tron.ITEM_CARDAPIO.Any(i => (i.CD_CLASSEITEMCARDAPIO == classe.CD_CLASSEITEMCARDAPIO))
                            orderby classe.CD_CLASSEITEMCARDAPIO
                            select new ClasseItemCardapioViewModel { CodClasse = classe.CD_CLASSEITEMCARDAPIO, DescricaoClasse = classe.DS_CLASSEITEMCARDAPIO };

                result.data = query.ToList();

                tron.Dispose();
                tron = null;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}