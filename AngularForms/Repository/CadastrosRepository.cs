using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;
using System.Drawing;

namespace BrasaoHamburgueria.Web.Repository
{
    public class CadastrosRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();

        #region Cadastros de opções extra
        public async Task<List<OpcaoExtraViewModel>> GetOpcoesExtra()
        {
            return _contexto.Extras.OrderBy(o => o.CodOpcaoExtra).Select(o => new OpcaoExtraViewModel { CodOpcaoExtra = o.CodOpcaoExtra, DescricaoOpcaoExtra = o.DescricaoOpcaoExtra, Preco = o.Preco }).ToList();
        }

        public async Task<OpcaoExtraViewModel> GravarOpcaoExtra(OpcaoExtraViewModel opcao, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var opcaoAlterar = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                if (opcaoAlterar != null)
                {
                    opcaoAlterar.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                    opcaoAlterar.Preco = opcao.Preco;

                    await _contexto.SaveChangesAsync();
                }

                return opcao;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var opcaoIncluir = new OpcaoExtra();
                if (opcao.CodOpcaoExtra <= 0)
                {
                    opcaoIncluir.CodOpcaoExtra = 1;
                    var cod = _contexto.Extras.Max(o => o.CodOpcaoExtra);
                    if (cod != null)
                    {
                        opcaoIncluir.CodOpcaoExtra = cod + 1;
                    }
                    opcao.CodOpcaoExtra = opcaoIncluir.CodOpcaoExtra;
                }
                else
                {
                    var valida = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma opção extra cadastrada com o código " + opcao.CodOpcaoExtra);
                    }

                    opcaoIncluir.CodOpcaoExtra = opcao.CodOpcaoExtra;
                }

                opcaoIncluir.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                opcaoIncluir.Preco = opcao.Preco;

                _contexto.Extras.Add(opcaoIncluir);

                await _contexto.SaveChangesAsync();

                return opcao;
            }

            return null;
        }

        public async Task<string> ExcluiOpcaoExtra(OpcaoExtraViewModel opcao)
        {
            if (_contexto.ExtrasItensPedidos.Where(i => i.CodOpcaoExtra == opcao.CodOpcaoExtra).Count() > 0)
            {
                return "Exclusão não permitida. Esta opção extra já foi utilizada em pedidos registrados na base.";
            }

            if (_contexto.ExtrasPermitidos.Where(i => i.CodOpcaoExtra == opcao.CodOpcaoExtra).Count() > 0)
            {
                return "Exclusão não permitida. Esta opção extra está associada a itens de cardápio.";
            }

            var opcaoExcluir = await _contexto.Extras.FindAsync(opcao.CodOpcaoExtra);

            if (opcaoExcluir != null)
            {
                _contexto.Extras.Remove(opcaoExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion

        #region Cadastros de observações
        public async Task<List<ObservacaoProducaoViewModel>> GetObservacoes()
        {
            return _contexto.ObservacoesProducao.OrderBy(o => o.CodObservacao).Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.CodObservacao, DescricaoObservacao = o.DescricaoObservacao }).ToList();
        }

        public async Task<ObservacaoProducaoViewModel> GravarObservacao(ObservacaoProducaoViewModel obs, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var obsAlterar = _contexto.ObservacoesProducao.Find(obs.CodObservacao);

                if (obsAlterar != null)
                {
                    obsAlterar.DescricaoObservacao = obs.DescricaoObservacao;

                    await _contexto.SaveChangesAsync();
                }

                return obs;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var obsIncluir = new ObservacaoProducao();
                if (obs.CodObservacao <= 0)
                {
                    obsIncluir.CodObservacao = 1;
                    var cod = _contexto.ObservacoesProducao.Max(o => o.CodObservacao);
                    if (cod != null)
                    {
                        obsIncluir.CodObservacao = cod + 1;
                    }
                    obs.CodObservacao = obsIncluir.CodObservacao;
                }
                else
                {
                    var valida = _contexto.ObservacoesProducao.Find(obs.CodObservacao);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma observação cadastrada com o código " + obs.CodObservacao);
                    }

                    obsIncluir.CodObservacao = obs.CodObservacao;
                }
                obsIncluir.DescricaoObservacao = obs.DescricaoObservacao;

                _contexto.ObservacoesProducao.Add(obsIncluir);

                await _contexto.SaveChangesAsync();

                return obs;
            }

            return null;
        }

        public async Task<string> ExcluiObservacao(ObservacaoProducaoViewModel obs)
        {
            if (_contexto.ObservacoesItensPedidos.Where(i => i.CodObservacao == obs.CodObservacao).Count() > 0)
            {
                return "Exclusão não permitida. Esta observação já foi utilizada em pedidos registrados na base.";
            }

            if (_contexto.ObservacoesPermitidas.Where(i => i.CodObservacao == obs.CodObservacao).Count() > 0)
            {
                return "Exclusão não permitida. Esta observação está associada a itens de cardápio.";
            }

            var obsExcluir = await _contexto.ObservacoesProducao.FindAsync(obs.CodObservacao);

            if (obsExcluir != null)
            {
                _contexto.ObservacoesProducao.Remove(obsExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion

        #region Impressoras de produção

        public async Task<List<ImpressoraProducaoViewModel>> GetImpressorasProducao()
        {
            return _contexto.ImpressorasProducao
                .OrderBy(o => o.CodImpressora)
                .Select(o => new ImpressoraProducaoViewModel
                {
                    CodImpressora = o.CodImpressora,
                    Descricao = o.Descricao,
                    Porta = o.Porta
                }).ToList();
        }

        public async Task<ImpressoraProducaoViewModel> GravarImpressoraProducao(ImpressoraProducaoViewModel imp, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var impAlterar = _contexto.ImpressorasProducao.Find(imp.CodImpressora);

                if (impAlterar != null)
                {
                    impAlterar.Descricao = imp.Descricao;
                    impAlterar.Porta = imp.Porta;

                    await _contexto.SaveChangesAsync();
                }

                return imp;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var impIncluir = new ImpressoraProducao();
                if (imp.CodImpressora <= 0)
                {
                    impIncluir.CodImpressora = 1;
                    var cod = _contexto.ImpressorasProducao.Max(o => o.CodImpressora);
                    if (cod != null)
                    {
                        impIncluir.CodImpressora = cod + 1;
                    }
                    imp.CodImpressora = impIncluir.CodImpressora;
                }
                else
                {
                    var valida = _contexto.ImpressorasProducao.Find(imp.CodImpressora);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma impressora cadastrada com o código " + imp.CodImpressora);
                    }

                    impIncluir.CodImpressora = imp.CodImpressora;
                }
                impIncluir.Descricao = imp.Descricao;
                impIncluir.Porta = imp.Porta;

                _contexto.ImpressorasProducao.Add(impIncluir);

                await _contexto.SaveChangesAsync();

                return imp;
            }

            return null;
        }

        public async Task<string> ExcluiImpressoraProducao(ImpressoraProducaoViewModel imp)
        {
            if (_contexto.ImpressorasItens.Where(i => i.CodImpressora == imp.CodImpressora).Count() > 0)
            {
                return "Exclusão não permitida. Esta impressora está associada a itens de cardápio.";
            }

            if (_contexto.Classes.Where(i => i.CodImpressoraPadrao == imp.CodImpressora).Count() > 0)
            {
                return "Exclusão não permitida. Esta impressora está associada a classes de itens de cardápio.";
            }

            var impExcluir = await _contexto.ImpressorasProducao.FindAsync(imp.CodImpressora);

            if (impExcluir != null)
            {
                _contexto.ImpressorasProducao.Remove(impExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }

        #endregion

        #region Cadastros de item de cardápio
        public async Task<List<ItemCardapioViewModel>> GetItensCardapio()
        {
            var lista = (
                from itens in _contexto.ItensCardapio
                from classes in _contexto.Classes.Where(c => c.CodClasse == itens.CodClasse)
                from complementos in _contexto.ComplementosItens
                    .Where(c => c.CodItemCardapio == itens.CodItemCardapio).DefaultIfEmpty()
                from imps in _contexto.ImpressorasItens
                    .Where(i => i.CodItemCardapio == itens.CodItemCardapio).Take(1).DefaultIfEmpty()
                select new ItemCardapioViewModel
                {
                    CodItemCardapio = itens.CodItemCardapio,
                    CodClasse = itens.CodClasse,
                    DescricaoClasse = classes.DescricaoClasse,
                    CodImpressoraProducao = imps.CodImpressora,
                    Complemento = new ComplementoItemCardapioViewModel
                    {
                        DescricaoLonga = complementos.DescricaoLonga,
                        Imagem = complementos.Imagem,
                        OrdemExibicao = complementos.OrdemExibicao
                    },
                    Nome = itens.Nome,
                    Preco = itens.Preco,
                    Sincronizar = itens.Sincronizar,
                    Ativo = itens.Ativo
                }
                ).ToList();

            foreach (var item in lista)
            {
                if (item.Complemento != null && !String.IsNullOrEmpty(item.Complemento.Imagem))
                {
                    item.Complemento.ImagemMini = item.Complemento.Imagem.Replace("img_item", "mini-img_item");
                }
            }

            return lista;
        }

        public async Task<string> ExcluiItemCardapio(ItemCardapioViewModel item)
        {
            if (_contexto.ItensPedidos.Where(i => i.CodItemCardapio == item.CodItemCardapio).Count() > 0)
            {
                return "Exclusão não permitida. Este item está associado a pedidos realizados.";
            }

            var itemExcluir = await _contexto.ItensCardapio.FindAsync(item.CodItemCardapio);

            if (itemExcluir != null)
            {
                var complementoExcluir = await _contexto.ComplementosItens.FindAsync(item.CodItemCardapio);

                if (complementoExcluir != null)
                {
                    _contexto.ComplementosItens.Remove(complementoExcluir);
                }

                var impressorasExcluir = _contexto.ImpressorasItens.Where(i => i.CodItemCardapio == item.CodItemCardapio).ToList();
                if (impressorasExcluir != null)
                {
                    foreach (var imp in impressorasExcluir)
                    {
                        _contexto.ImpressorasItens.Remove(imp);
                    }
                }

                _contexto.ItensCardapio.Remove(itemExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }

        public async Task<ItemCardapioViewModel> GravarItemCardapio(ItemCardapioViewModel item, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var itemAlterar = _contexto.ItensCardapio.Find(item.CodItemCardapio);

                if (itemAlterar != null)
                {
                    if (item.Preco <= 0)
                    {
                        throw new Exception("O preço informado não é valido. Informe um número maior que zero.");
                    }

                    itemAlterar.Nome = item.Nome;
                    itemAlterar.Preco = item.Preco;
                    itemAlterar.Sincronizar = item.Sincronizar;
                    itemAlterar.Ativo = item.Ativo;
                    itemAlterar.CodClasse = item.CodClasse;

                    var complementoAlterar = _contexto.ComplementosItens.Find(item.CodItemCardapio);

                    if (complementoAlterar != null)
                    {
                        complementoAlterar.DescricaoLonga = item.Complemento.DescricaoLonga;
                        complementoAlterar.Imagem = item.Complemento.Imagem;
                        if (item.Complemento.OrdemExibicao != null)
                        {
                            complementoAlterar.OrdemExibicao = item.Complemento.OrdemExibicao.Value;
                        }
                    }

                    var impressorasProducaoItem = _contexto.ImpressorasItens.Where(i => i.CodItemCardapio == item.CodItemCardapio).ToList();
                    if (impressorasProducaoItem != null)
                    {
                        foreach (var imp in impressorasProducaoItem)
                        {
                            _contexto.ImpressorasItens.Remove(imp);
                        }
                    }

                    if (item.CodImpressoraProducao != null)
                    {
                        var impressoraProducaoItem = new ItemCardapioImpressora();
                        impressoraProducaoItem.CodItemCardapio = item.CodItemCardapio;
                        impressoraProducaoItem.CodImpressora = item.CodImpressoraProducao.Value;

                        _contexto.ImpressorasItens.Add(impressoraProducaoItem);
                    }

                    await _contexto.SaveChangesAsync();
                }

                return item;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var itemIncluir = new ItemCardapio();
                if (item.CodItemCardapio <= 0)
                {
                    itemIncluir.CodItemCardapio = 1;
                    var cod = _contexto.ItensCardapio.Max(o => o.CodItemCardapio);
                    if (cod != null)
                    {
                        itemIncluir.CodItemCardapio = cod + 1;
                    }
                    item.CodItemCardapio = itemIncluir.CodItemCardapio;
                }
                else
                {
                    var valida = _contexto.ItensCardapio.Find(item.CodItemCardapio);

                    if (valida != null)
                    {
                        throw new Exception("Já existe um item de cardápio cadastrado com o código " + item.CodItemCardapio);
                    }

                    itemIncluir.CodItemCardapio = item.CodItemCardapio;
                }

                if (item.Preco <= 0)
                {
                    throw new Exception("O preço informado não é valido. Informe um número maior que zero.");
                }

                itemIncluir.Nome = item.Nome;
                itemIncluir.Preco = item.Preco;
                itemIncluir.Sincronizar = item.Sincronizar;
                itemIncluir.Ativo = item.Ativo;
                itemIncluir.CodClasse = item.CodClasse;

                ComplementoItemCardapio complementoIncluir = new ComplementoItemCardapio();

                complementoIncluir.CodItemCardapio = item.CodItemCardapio;
                complementoIncluir.DescricaoLonga = item.Complemento.DescricaoLonga;
                complementoIncluir.Imagem = item.Complemento.Imagem;
                if (item.Complemento.OrdemExibicao != null)
                {
                    complementoIncluir.OrdemExibicao = item.Complemento.OrdemExibicao.Value;
                }

                _contexto.ItensCardapio.Add(itemIncluir);
                _contexto.ComplementosItens.Add(complementoIncluir);

                if (item.CodImpressoraProducao != null)
                {
                    var impressoraProducaoItem = new ItemCardapioImpressora();
                    impressoraProducaoItem.CodItemCardapio = item.CodItemCardapio;
                    impressoraProducaoItem.CodImpressora = item.CodImpressoraProducao.Value;

                    _contexto.ImpressorasItens.Add(impressoraProducaoItem);
                }

                await _contexto.SaveChangesAsync();

                return item;
            }

            return null;
        }

        public string GravarImagemItemCardapio(HttpPostedFileBase file, int codItemCardapio, string serverPath)
        {
            var extensao = file.FileName.Split('.')[1].ToString();
            var imgPath = serverPath + @"Content\img\itens_cardapio\" + "img_item" + codItemCardapio.ToString() + "." + extensao;
            file.SaveAs(imgPath);

            if (file.ContentLength > 500000)
            {
                throw new Exception("A imagem deve ter no máximo 500Kb.");
            }

            var thumbPath = serverPath + @"Content\img\itens_cardapio\" + "mini-img_item" + codItemCardapio.ToString() + "." + extensao;

            //cria miniatura

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            Image image = Image.FromFile(imgPath);

            int height = 150;
            int width = Convert.ToInt32(height * (Convert.ToDecimal(image.Width) / Convert.ToDecimal(image.Height)));

            Image thumb = image.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            thumb.Save(thumbPath);

            image.Dispose();

            //grava caminho da imagem no registro da classe
            var complemento = _contexto.ComplementosItens.Find(codItemCardapio);

            if (complemento != null)
            {
                complemento.Imagem = @"Content/img/itens_cardapio/" + "img_item" + codItemCardapio.ToString() + "." + extensao;
                _contexto.SaveChanges();

                return complemento.Imagem;
            }

            return "";
        }

        public void RemoverImagemItemCardapio(ItemCardapioViewModel item, string serverPath)
        {
            var complementoDb = _contexto.ComplementosItens.Find(item.CodItemCardapio);

            if (complementoDb == null || String.IsNullOrEmpty(complementoDb.Imagem))
            {
                return;
            }

            var array = complementoDb.Imagem.Split('/');
            var imagem = serverPath + @"Content\img\itens_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            array = item.Complemento.ImagemMini.Split('/');
            imagem = serverPath + @"Content\img\classes_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            complementoDb.Imagem = null;
            _contexto.SaveChanges();

        }

        #endregion

        #region Cadastros de classes de item de cardápio
        public async Task<List<ClasseItemCardapioViewModel>> GetClassesItemCardapio()
        {
            var lista = _contexto.Classes
                .OrderBy(o => o.CodClasse)
                .Select(o => new ClasseItemCardapioViewModel
                {
                    CodClasse = o.CodClasse,
                    DescricaoClasse = o.DescricaoClasse,
                    CodImpressoraPadrao = o.CodImpressoraPadrao,
                    Sincronizar = o.Sincronizar,
                    Imagem = o.Imagem,
                    OrdemExibicao = o.OrdemExibicao,
                    DescricaoImpressoraPadrao = _contexto.ImpressorasProducao.Where(i => i.CodImpressora == o.CodImpressoraPadrao).FirstOrDefault().Descricao
                }).ToList();

            foreach (var classe in lista)
            {
                if (!String.IsNullOrEmpty(classe.Imagem))
                {
                    classe.ImagemMini = classe.Imagem.Replace("img_classe", "mini-img_classe");
                }
            }

            return lista;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        public void RemoverImagemClasse(ClasseItemCardapioViewModel classe, string serverPath)
        {
            var classeDb = _contexto.Classes.Find(classe.CodClasse);

            var array = classe.Imagem.Split('/');
            var imagem = serverPath + @"Content\img\classes_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            array = classe.ImagemMini.Split('/');
            imagem = serverPath + @"Content\img\classes_cardapio\" + array[array.Length - 1];

            System.IO.File.Delete(imagem);

            if (classeDb != null)
            {

                classeDb.Imagem = null;
                _contexto.SaveChanges();

            }
        }

        public string GravarImagemClasse(HttpPostedFileBase file, int codClasse, string serverPath)
        {
            var extensao = file.FileName.Split('.')[1].ToString();
            var imgPath = serverPath + @"Content\img\classes_cardapio\" + "img_classe" + codClasse.ToString() + "." + extensao;
            file.SaveAs(imgPath);

            if (file.ContentLength > 500000)
            {
                throw new Exception("A imagem deve ter no máximo 500Kb.");
            }

            var thumbPath = serverPath + @"Content\img\classes_cardapio\" + "mini-img_classe" + codClasse.ToString() + "." + extensao;

            //cria miniatura

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            Image image = Image.FromFile(imgPath);

            int height = 150;
            int width = Convert.ToInt32(height * (Convert.ToDecimal(image.Width) / Convert.ToDecimal(image.Height)));

            Image thumb = image.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            thumb.Save(thumbPath);

            image.Dispose();

            //grava caminho da imagem no registro da classe
            var classe = _contexto.Classes.Find(codClasse);

            if (classe != null)
            {
                classe.Imagem = @"Content/img/classes_cardapio/" + "img_classe" + codClasse.ToString() + "." + extensao;
                _contexto.SaveChanges();

                return classe.Imagem;
            }

            return "";
        }

        public async Task<ClasseItemCardapioViewModel> GravarClasseItemCardapio(ClasseItemCardapioViewModel classe, String modoCadastro)
        {
            if (modoCadastro == "A") //alteração
            {
                var classeAlterar = _contexto.Classes.Find(classe.CodClasse);

                if (classeAlterar != null)
                {
                    classeAlterar.DescricaoClasse = classe.DescricaoClasse;
                    classeAlterar.CodImpressoraPadrao = classe.CodImpressoraPadrao;
                    classeAlterar.Imagem = classe.Imagem;
                    classeAlterar.OrdemExibicao = classe.OrdemExibicao;
                    classeAlterar.Sincronizar = classe.Sincronizar;

                    await _contexto.SaveChangesAsync();
                }

                return classe;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var classeIncluir = new ClasseItemCardapio();
                if (classe.CodClasse <= 0)
                {
                    classeIncluir.CodClasse = 1;
                    var cod = _contexto.Classes.Max(o => o.CodClasse);
                    if (cod != null)
                    {
                        classeIncluir.CodClasse = cod + 1;
                    }
                    classe.CodClasse = classeIncluir.CodClasse;
                }
                else
                {
                    var valida = _contexto.Classes.Find(classe.CodClasse);

                    if (valida != null)
                    {
                        throw new Exception("Já existe uma classe de item de cardápio cadastrada com o código " + classe.CodClasse);
                    }

                    classeIncluir.CodClasse = classe.CodClasse;
                }
                classeIncluir.DescricaoClasse = classe.DescricaoClasse;
                classeIncluir.CodImpressoraPadrao = classe.CodImpressoraPadrao;
                classeIncluir.Imagem = classe.Imagem;
                classeIncluir.OrdemExibicao = classe.OrdemExibicao;
                classeIncluir.Sincronizar = classe.Sincronizar;

                _contexto.Classes.Add(classeIncluir);

                await _contexto.SaveChangesAsync();

                return classe;
            }

            return null;
        }

        public async Task<string> ExcluiClasseItemCardapio(ClasseItemCardapioViewModel classe)
        {
            if (_contexto.ItensCardapio.Where(i => i.CodClasse == classe.CodClasse).Count() > 0)
            {
                return "Exclusão não permitida. Esta classe está associada a itens de cardápio.";
            }

            var classeExcluir = await _contexto.Classes.FindAsync(classe.CodClasse);

            if (classeExcluir != null)
            {
                _contexto.Classes.Remove(classeExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }
        #endregion

    }
}