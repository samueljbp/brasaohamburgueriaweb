using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BrasaoHamburgueria.Web.Context;
using BrasaoHamburgueria.Model;
using System.Threading.Tasks;
using System.Drawing;
using System.Data.Entity;
using BrasaoHamburgueria.Web.Helpers;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Configuration;
using BrasaoHamburgueria.Helper;
using System.IO;

namespace BrasaoHamburgueria.Web.Repository
{
    public class CadastrosRepository
    {
        private BrasaoContext _contexto = new BrasaoContext();

        #region Empresas

        public void RemoverImagem(string serverPath, string fileUrl, string fileMiniUrl)
        {

            var imagem = serverPath + fileUrl.Replace(@"/", @"\");
            System.IO.File.Delete(imagem);

            imagem = serverPath + fileMiniUrl.Replace(@"/", @"\");
            System.IO.File.Delete(imagem);
        }

        private void FillImagensEmpresa(List<EmpresaViewModel> empresas, string serverPath)
        {
            //var serverPath = @"C:\Users\sjbp\source\repos\brasaohamburgueriaweb\AngularForms\";
            var imgPath = System.Configuration.ConfigurationManager.AppSettings["CaminhoImagensEmpresa"].ToString();

            foreach (var empresa in empresas)
            {
                var fullPath = serverPath + imgPath + empresa.CodEmpresa + @"\";

                var imagens = new List<ImagemInstitucionalViewModel>();

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    return;
                }

                var files = Directory.GetFiles(fullPath);
                foreach (var file in files.Where(x => x.Contains("mini")))
                {
                    ImagemInstitucionalViewModel img = new ImagemInstitucionalViewModel();
                    img.ImagemMini = file;
                    imagens.Add(img);
                }

                foreach (var img in imagens)
                {
                    var aux = img.ImagemMini.Split('_');
                    var numero = aux[aux.Length - 1].Split('.')[0];
                    var file = files.Where(x => x.Contains("img_empresa_" + numero + ".")).FirstOrDefault();
                    img.Imagem = file;

                    img.Imagem = imgPath.Replace(@"\", @"/") + empresa.CodEmpresa + @"/" + img.Imagem.Split('\\')[img.Imagem.Split('\\').Length - 1];
                    img.ImagemMini = imgPath.Replace(@"\", @"/") + empresa.CodEmpresa + @"/" + img.ImagemMini.Split('\\')[img.ImagemMini.Split('\\').Length - 1];
                }

                empresa.ImagensInstitucionais = imagens;
            }

        }

        public string GravarImagem(HttpPostedFileBase file, string serverPath, string imgPath, string fileName, int maxSize)
        {
            var extensao = file.FileName.Split('.')[1].ToString();
            var fullPath = serverPath + imgPath + fileName + "." + extensao;
            file.SaveAs(fullPath);

            if (file.ContentLength > maxSize)
            {
                throw new Exception("A imagem deve ter no máximo " + (maxSize / 1000).ToString() + "Kb.");
            }

            var thumbPath = serverPath + imgPath + "mini-" + fileName + "." + extensao;

            //cria miniatura

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            Image image = Image.FromFile(fullPath);

            int height = 150;
            int width = Convert.ToInt32(height * (Convert.ToDecimal(image.Width) / Convert.ToDecimal(image.Height)));

            Image thumb = image.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            thumb.Save(thumbPath);

            image.Dispose();

            return imgPath.Replace(@"\", @"/") + fileName + "." + extensao;
        }

        public void RemoverLogoEmpresa(string serverPath, EmpresaViewModel empresa)
        {
            try
            {
                var empresaDb = _contexto.Empresas.Find(empresa.CodEmpresa);

                if (empresaDb == null)
                {
                    throw new Exception("O registro da empresa " + empresa.CodEmpresa + " não foi encontrado na base de dados.");
                }

                RemoverImagem(serverPath, empresa.Logomarca, empresa.LogomarcaMini);

                empresaDb.Logomarca = null;
                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GravarLogoEmpresa(HttpPostedFileBase file, string serverPath, int codEmpresa)
        {
            var retorno = "";

            try
            {
                var empresa = _contexto.Empresas.Find(codEmpresa);

                if (empresa == null)
                {
                    throw new Exception("O registro da empresa " + codEmpresa + " não foi encontrado na base de dados.");
                }

                var caminhoLogo = ConfigurationManager.AppSettings["CaminhoPadraoImagens"].ToString();
                var caminhoImagem = GravarImagem(file, serverPath, caminhoLogo, "img_logo" + codEmpresa, 300000);
                empresa.Logomarca = caminhoImagem;
                retorno = empresa.Logomarca;
                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public void RemoverFundoPublicoEmpresa(string serverPath, EmpresaViewModel empresa)
        {
            try
            {
                var empresaDb = _contexto.Empresas.Find(empresa.CodEmpresa);

                if (empresaDb == null)
                {
                    throw new Exception("O registro da empresa " + empresa.CodEmpresa + " não foi encontrado na base de dados.");
                }

                RemoverImagem(serverPath, empresa.ImagemBackgroundPublica, empresa.ImagemBackgroundPublicaMini);

                empresaDb.ImagemBackgroundPublica = null;
                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GravarFundoPublicoEmpresa(HttpPostedFileBase file, string serverPath, int codEmpresa)
        {
            var retorno = "";

            try
            {
                var empresa = _contexto.Empresas.Find(codEmpresa);

                if (empresa == null)
                {
                    throw new Exception("O registro da empresa " + codEmpresa + " não foi encontrado na base de dados.");
                }

                var caminhoFundo = ConfigurationManager.AppSettings["CaminhoPadraoImagens"].ToString();
                var caminhoImagem = GravarImagem(file, serverPath, caminhoFundo, "img_bg_publica" + codEmpresa, 1000000);
                empresa.ImagemBackgroundPublica = caminhoImagem;
                retorno = empresa.ImagemBackgroundPublica;
                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public void RemoverFundoAutenticadoEmpresa(string serverPath, EmpresaViewModel empresa)
        {
            try
            {
                var empresaDb = _contexto.Empresas.Find(empresa.CodEmpresa);

                if (empresaDb == null)
                {
                    throw new Exception("O registro da empresa " + empresa.CodEmpresa + " não foi encontrado na base de dados.");
                }

                RemoverImagem(serverPath, empresa.ImagemBackgroundAutenticada, empresa.ImagemBackgroundAutenticadaMini);

                empresaDb.ImagemBackgroundAutenticada = null;
                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GravarFundoAutenticadoEmpresa(HttpPostedFileBase file, string serverPath, int codEmpresa)
        {
            var retorno = "";

            try
            {
                var empresa = _contexto.Empresas.Find(codEmpresa);

                if (empresa == null)
                {
                    throw new Exception("O registro da empresa " + codEmpresa + " não foi encontrado na base de dados.");
                }

                var caminhoFundo = ConfigurationManager.AppSettings["CaminhoPadraoImagens"].ToString();
                var caminhoImagem = GravarImagem(file, serverPath, caminhoFundo, "img_bg_autenticada" + codEmpresa, 1000000);
                empresa.ImagemBackgroundAutenticada = caminhoImagem;
                retorno = empresa.ImagemBackgroundAutenticada;
                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public List<ImagemInstitucionalViewModel> GravarImagemInstitucional(HttpPostedFileBase file, string serverPath, int codEmpresa)
        {
            var retorno = new List<ImagemInstitucionalViewModel>();

            try
            {
                List<EmpresaViewModel> emps = new List<EmpresaViewModel>();
                var emp = new EmpresaViewModel();
                emp.CodEmpresa = codEmpresa;
                emps.Add(emp);
                FillImagensEmpresa(emps, serverPath);
                var imagens = emps[0].ImagensInstitucionais;

                var imgNumber = 1;
                if (imagens != null && imagens.Count > 0)
                {
                    imgNumber = imagens.Count + 1;
                }

                GravarImagem(file, serverPath, ConfigurationManager.AppSettings["CaminhoPadraoImagens"].ToString() + @"empresas\" + codEmpresa + @"\", "img_empresa_" + imgNumber, 1000000);

                List<EmpresaViewModel> empresas = AsyncHelpers.RunSync<List<EmpresaViewModel>>(() => this.GetEmpresas());
                FillImagensEmpresa(empresas, serverPath);

                retorno = empresas.Where(e => e.CodEmpresa == codEmpresa).FirstOrDefault().ImagensInstitucionais;

                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public void RemoverImagemInstitucionalEmpresa(string serverPath, EmpresaViewModel empresa, string arquivo)
        {
            try
            {
                var imagemMini = empresa.ImagensInstitucionais.Where(i => i.Imagem == arquivo).FirstOrDefault().ImagemMini;

                RemoverImagem(serverPath, arquivo, imagemMini);

                SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                _contexto.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EstadoViewModel>> GetEstados()
        {
            var lista = await (
                from cids in _contexto.Cidades
                select new EstadoViewModel
                {
                    Sigla = cids.Estado,
                    Nome = cids.Estado
                }
                ).Distinct().OrderBy(c => c.Sigla).ToListAsync();

            return lista;
        }

        public async Task<List<CidadeViewModel>> GetCidades(string siglaEstado)
        {
            var lista = await (
                from cids in _contexto.Cidades
                where cids.Estado == (siglaEstado != null ? siglaEstado : cids.Estado)
                select new CidadeViewModel
                {
                    CodCidade = cids.CodCidade,
                    Nome = cids.Nome,
                    Estado = cids.Estado
                }
                ).Distinct().OrderBy(c => c.Nome).ToListAsync();

            return lista;
        }

        public async Task<List<BairroViewModel>> GetBairros(int? codCidade)
        {
            var lista = await (
                from bairros in _contexto.Bairros
                where bairros.CodCidade == (codCidade != null ? codCidade.Value : bairros.CodCidade)
                select new BairroViewModel
                {
                    CodBairro = bairros.CodBairro,
                    Nome = bairros.Nome
                }
                ).Distinct().OrderBy(c => c.Nome).ToListAsync();

            return lista;
        }

        public async Task<EmpresaViewModel> GravarEmpresa(EmpresaViewModel empresa, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                if (modoCadastro == "A") //alteração
                {
                    var empresaAlterar = _contexto.Empresas.Find(empresa.CodEmpresa);

                    if (empresaAlterar != null)
                    {
                        empresaAlterar.RazaoSocial = empresa.RazaoSocial;
                        empresaAlterar.NomeFantasia = empresa.NomeFantasia;
                        empresaAlterar.CNPJ = BrasaoUtil.FormatCNPJ(BrasaoUtil.SemFormatacao(empresa.CNPJ));
                        empresaAlterar.CodBairro = empresa.CodBairro;
                        empresaAlterar.CodEmpresaMatriz = empresa.CodEmpresaMatriz;
                        empresaAlterar.Complemento = empresa.Complemento;
                        empresaAlterar.CorDestaque = empresa.CorDestaque;
                        empresaAlterar.CorPrincipal = empresa.CorPrincipal;
                        empresaAlterar.CorPrincipalContraste = empresa.CorPrincipalContraste;
                        empresaAlterar.CorSecundaria = empresa.CorSecundaria;
                        empresaAlterar.TextoInstitucional = empresa.TextoInstitucional;
                        empresaAlterar.Email = empresa.Email;
                        empresaAlterar.EmpresaAtiva = empresa.EmpresaAtiva;
                        empresaAlterar.Facebook = empresa.Facebook;
                        empresaAlterar.ImagemBackgroundAutenticada = empresa.ImagemBackgroundAutenticada;
                        empresaAlterar.ImagemBackgroundPublica = empresa.ImagemBackgroundPublica;
                        empresaAlterar.InscricaoEstadual = empresa.InscricaoEstadual;
                        empresaAlterar.Logomarca = empresa.Logomarca;
                        empresaAlterar.Logradouro = empresa.Logradouro;
                        empresaAlterar.Numero = empresa.Numero;
                        empresaAlterar.Telefone = empresa.Telefone;

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();

                        BrasaoHamburgueria.Web.Helpers.SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);
                    }

                    return empresa;
                }
                else if (modoCadastro == "I") //inclusão
                {
                    var empresaIncluir = new Empresa();
                    if (empresa.CodEmpresa <= 0)
                    {
                        empresaIncluir.CodEmpresa = 1;

                        var cod = _contexto.Empresas.Select(o => o.CodEmpresa).DefaultIfEmpty(-1).Max();
                        if (cod > 0)
                        {
                            empresaIncluir.CodEmpresa = cod + 1;
                        }

                        empresa.CodEmpresa = empresaIncluir.CodEmpresa;
                    }
                    else
                    {
                        var valida = _contexto.Empresas.Find(empresa.CodEmpresa);

                        if (valida != null)
                        {
                            throw new Exception("Já existe uma empresa cadastrada com o código " + empresa.CodEmpresa);
                        }

                        empresaIncluir.CodEmpresa = empresa.CodEmpresa;
                    }

                    empresaIncluir.RazaoSocial = empresa.RazaoSocial;
                    empresaIncluir.NomeFantasia = empresa.NomeFantasia;
                    empresaIncluir.CNPJ = BrasaoUtil.FormatCNPJ(BrasaoUtil.SemFormatacao(empresa.CNPJ));
                    empresaIncluir.CodBairro = empresa.CodBairro;
                    empresaIncluir.CodEmpresaMatriz = empresa.CodEmpresaMatriz;
                    empresaIncluir.Complemento = empresa.Complemento;
                    empresaIncluir.CorDestaque = empresa.CorDestaque;
                    empresaIncluir.CorPrincipal = empresa.CorPrincipal;
                    empresaIncluir.CorPrincipalContraste = empresa.CorPrincipalContraste;
                    empresaIncluir.CorSecundaria = empresa.CorSecundaria;
                    empresaIncluir.TextoInstitucional = empresa.TextoInstitucional;
                    empresaIncluir.Email = empresa.Email;
                    empresaIncluir.EmpresaAtiva = empresa.EmpresaAtiva;
                    empresaIncluir.Facebook = empresa.Facebook;
                    empresaIncluir.ImagemBackgroundAutenticada = empresa.ImagemBackgroundAutenticada;
                    empresaIncluir.ImagemBackgroundPublica = empresa.ImagemBackgroundPublica;
                    empresaIncluir.InscricaoEstadual = empresa.InscricaoEstadual;
                    empresaIncluir.Logomarca = empresa.Logomarca;
                    empresaIncluir.Logradouro = empresa.Logradouro;
                    empresaIncluir.Numero = empresa.Numero;
                    empresaIncluir.Telefone = empresa.Telefone;

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    BrasaoHamburgueria.Web.Helpers.SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.Empresas);

                    return empresa;
                }
            }

            return null;
        }

        public async Task<List<EmpresaViewModel>> GetEmpresas()
        {
            var lista = (
                from emps in _contexto.Empresas
                join bairros in _contexto.Bairros on emps.CodBairro equals bairros.CodBairro
                join cidades in _contexto.Cidades on bairros.CodCidade equals cidades.CodCidade
                where SessionData.EmpresasInt.Contains(emps.CodEmpresa)
                select new EmpresaViewModel
                {
                    CodEmpresa = emps.CodEmpresa,
                    RazaoSocial = emps.RazaoSocial,
                    NomeFantasia = emps.NomeFantasia,
                    InscricaoEstadual = emps.InscricaoEstadual,
                    Logomarca = emps.Logomarca,
                    CNPJ = emps.CNPJ,
                    CodBairro = emps.CodBairro,
                    NomeBairro = bairros.Nome,
                    CodCidade = bairros.CodCidade,
                    NomeCidade = cidades.Nome,
                    CodEmpresaMatriz = emps.CodEmpresaMatriz,
                    Complemento = emps.Complemento,
                    Estado = cidades.Estado,
                    Logradouro = emps.Logradouro,
                    Numero = emps.Numero,
                    Telefone = emps.Telefone,
                    Email = emps.Email,
                    Facebook = emps.Facebook,
                    ImagemBackgroundAutenticada = emps.ImagemBackgroundAutenticada,
                    ImagemBackgroundPublica = emps.ImagemBackgroundPublica,
                    CorPrincipal = emps.CorPrincipal,
                    CorSecundaria = emps.CorSecundaria,
                    CorPrincipalContraste = emps.CorPrincipalContraste,
                    CorDestaque = emps.CorDestaque,
                    TextoInstitucional = emps.TextoInstitucional,
                    EhFilial = (emps.CodEmpresaMatriz == null),
                    EmpresaAtiva = emps.EmpresaAtiva
                }
                ).ToList();

            foreach (var item in lista)
            {
                if (!String.IsNullOrEmpty(item.Logomarca))
                {
                    item.LogomarcaMini = item.Logomarca.Replace("img_logo", "mini-img_logo");
                }

                if (!String.IsNullOrEmpty(item.ImagemBackgroundPublica))
                {
                    item.ImagemBackgroundPublicaMini = item.ImagemBackgroundPublica.Replace("img_bg_publica", "mini-img_bg_publica");
                }

                if (!String.IsNullOrEmpty(item.ImagemBackgroundAutenticada))
                {
                    item.ImagemBackgroundAutenticadaMini = item.ImagemBackgroundAutenticada.Replace("img_bg_autenticada", "mini-img_bg_autenticada");
                }
            }

            FillImagensEmpresa(lista, HttpContext.Current.Server.MapPath("~").ToString());

            return lista;
        }


        public async Task<string> ExcluiEmpresa(EmpresaViewModel empresa)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var empresaExcluir = await _contexto.Empresas.FindAsync(empresa.CodEmpresa);

                    if (empresaExcluir != null)
                    {
                        _contexto.Empresas.Remove(empresaExcluir);
                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return "Registro não encontrado na base de dados.";
                    }
                }
                catch (DbUpdateException ex)
                {
                    var sqlex = ex.InnerException.InnerException as SqlException;

                    if (sqlex != null)
                    {
                        switch (sqlex.Number)
                        {
                            case 547: throw new Exception("Não foi possível excluir a empresa pois ela já está associada a registros na base. Como sugestão utilize a opção de inativar o registro.");
                            default: throw sqlex; //otra excepcion que no controlo.


                        }
                    }

                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return "";
        }


        #endregion  

        #region Formas de pagamento
        public async Task<List<FormaPagamentoViewModel>> GetFormasPagamento()
        {
            return await _contexto.FormasPagamento.OrderBy(o => o.CodFormaPagamento).Select(o => new FormaPagamentoViewModel
            {
                CodFormaPagamento = o.CodFormaPagamento,
                DescricaoFormaPagamento = o.DescricaoFormaPagamento
            }).ToListAsync();
        }
        #endregion  

        #region Bandeiras de cartão
        public async Task<List<BandeiraCartaoViewModel>> GetBandeirasCartao()
        {
            return await _contexto.BandeirasCartao.OrderBy(o => o.CodBandeiraCartao).Select(o => new BandeiraCartaoViewModel
            {
                CodBandeiraCartao = o.CodBandeiraCartao,
                DescricaoBandeiraCartao = o.DescricaoBandeiraCartao
            }).ToListAsync();
        }
        #endregion  

        #region Entregadores

        public async Task<List<EntregadorViewModel>> GetEntregadores(int codEmpresa)
        {
            return await _contexto.Entregadores.Include(e => e.Empresa)
                .Where(o => (o.CodEmpresa != null ? o.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(o.CodEmpresa != null ? o.CodEmpresa.Value : 0)))
                .OrderBy(o => o.CodEntregador).Select(o => new EntregadorViewModel
                {
                    CodEntregador = o.CodEntregador,
                    Nome = o.Nome,
                    Sexo = o.Sexo,
                    CPF = o.CPF,
                    Email = o.Email,
                    TelefoneCelular = o.TelefoneCelular,
                    TelefoneFixo = o.TelefoneFixo,
                    EnderecoCompleto = o.EnderecoCompleto,
                    Observacao = o.Observacao,
                    OrdemAcionamento = o.OrdemAcionamento,
                    ValorPorEntrega = o.ValorPorEntrega,
                    CodEmpresa = o.CodEmpresa,
                    NomeEmpresa = o.Empresa.NomeFantasia
                }).ToListAsync();
        }

        public async Task<EntregadorViewModel> GravarEntregador(EntregadorViewModel entregador, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                if (modoCadastro == "A") //alteração
                {
                    var entregadorAlterar = _contexto.Entregadores.Find(entregador.CodEntregador);

                    if (entregadorAlterar != null)
                    {
                        entregadorAlterar.CodEmpresa = entregador.CodEmpresa;
                        entregadorAlterar.Nome = entregador.Nome;
                        entregadorAlterar.Sexo = entregador.Sexo;
                        entregadorAlterar.CPF = entregador.CPF;
                        entregadorAlterar.Email = entregador.Email;
                        entregadorAlterar.TelefoneCelular = entregador.TelefoneCelular;
                        entregadorAlterar.TelefoneFixo = entregador.TelefoneFixo;
                        entregadorAlterar.EnderecoCompleto = entregador.EnderecoCompleto;
                        entregadorAlterar.Observacao = entregador.Observacao;
                        entregadorAlterar.OrdemAcionamento = entregador.OrdemAcionamento;
                        entregadorAlterar.ValorPorEntrega = entregador.ValorPorEntrega;

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }

                    return entregador;
                }
                else if (modoCadastro == "I") //inclusão
                {
                    var entregadorIncluir = new Entregador();
                    if (entregador.CodEntregador <= 0)
                    {
                        entregadorIncluir.CodEntregador = 1;

                        var cod = _contexto.Entregadores.Select(o => o.CodEntregador).DefaultIfEmpty(-1).Max();
                        if (cod > 0)
                        {
                            entregadorIncluir.CodEntregador = cod + 1;
                        }

                        entregador.CodEntregador = entregadorIncluir.CodEntregador;
                    }
                    else
                    {
                        var valida = _contexto.Entregadores.Find(entregador.CodEntregador);

                        if (valida != null)
                        {
                            throw new Exception("Já existe um entregador cadastrado com o código " + entregador.CodEntregador);
                        }

                        entregadorIncluir.CodEntregador = entregador.CodEntregador;
                    }

                    entregadorIncluir.CodEmpresa = entregador.CodEmpresa;
                    entregadorIncluir.Nome = entregador.Nome;
                    entregadorIncluir.Sexo = entregador.Sexo;
                    entregadorIncluir.CPF = entregador.CPF;
                    entregadorIncluir.Email = entregador.Email;
                    entregadorIncluir.TelefoneCelular = entregador.TelefoneCelular;
                    entregadorIncluir.TelefoneFixo = entregador.TelefoneFixo;
                    entregadorIncluir.EnderecoCompleto = entregador.EnderecoCompleto;
                    entregadorIncluir.Observacao = entregador.Observacao;
                    entregadorIncluir.OrdemAcionamento = entregador.OrdemAcionamento;
                    entregadorIncluir.ValorPorEntrega = entregador.ValorPorEntrega;

                    _contexto.Entregadores.Add(entregadorIncluir);

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    return entregador;
                }
            }

            return null;
        }

        public async Task<string> ExcluiEntregador(EntregadorViewModel entregador)
        {
            if (_contexto.Pedidos.Where(i => i.CodEntregador == entregador.CodEntregador).Count() > 0)
            {
                return "Exclusão não permitida. Este entregador está associado a pedidos registrados na base.";
            }

            var entregadorExcluir = await _contexto.Entregadores.FindAsync(entregador.CodEntregador);

            if (entregadorExcluir != null)
            {
                _contexto.Entregadores.Remove(entregadorExcluir);
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return "Registro não encontrado na base de dados.";
            }

            return "";
        }

        #endregion

        #region Cadastros de combo de cardápio

        public List<ComboViewModel> GetCombosDB(bool combosDoDia, int? codEmpresa)
        {
            var combos = (from cmb in _contexto.Combos
                               .Include(c => c.Itens)
                               .Include(c => c.Empresa)
                               .Include(c => c.Itens.Select(i => i.Item))
                               .Where(c => (c.CodEmpresa != null ? c.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(c.CodEmpresa != null ? c.CodEmpresa.Value : 0)))
                          select new ComboViewModel
                          {
                              CodCombo = cmb.CodCombo,
                              CodEmpresa = cmb.CodEmpresa,
                              NomeEmpresa = cmb.Empresa.NomeFantasia,
                              Nome = cmb.NomeCombo,
                              Descricao = cmb.DescricaoCombo,
                              Preco = cmb.PrecoCombo,
                              PrecoCombo = cmb.PrecoCombo,
                              Ativo = cmb.Ativo,
                              DataHoraInicio = cmb.DataHoraInicio,
                              DataHoraFim = cmb.DataHoraFim,
                              Itens = cmb.Itens.Select(i => new ComboItemCardapioViewModel
                              {
                                  CodCombo = cmb.CodCombo,
                                  CodItemCardapio = i.CodItemCardapio,
                                  Quantidade = i.Quantidade,
                                  Nome = i.Item.Nome
                              }).ToList(),
                              DiasAssociados = cmb.DiasAssociados.Select(d => new DiaSemanaViewModel
                              {
                                  NumDiaSemana = d.DiaSemana
                              }).ToList(),
                          }
                                ).OrderBy(c => c.CodCombo).ToList();

            foreach (var combo in combos)
            {
                combo.DataInicio = combo.DataHoraInicio.ToString("dd/MM/yyyy");
                combo.HoraInicio = combo.DataHoraInicio.ToString("HH:mm");
                combo.DataFim = combo.DataHoraFim.ToString("dd/MM/yyyy");
                combo.HoraFim = combo.DataHoraFim.ToString("HH:mm");
            }

            if (combosDoDia)
            {
                var numDiaHoje = (int)DateTime.Now.DayOfWeek;

                return (from cmbs in combos
                        where cmbs.Ativo &&
                              cmbs.DataHoraInicio <= DateTime.Now &&
                              cmbs.DataHoraFim >= DateTime.Now &&
                              cmbs.DiasAssociados.Select(d => d.NumDiaSemana).Contains(numDiaHoje)
                        select cmbs).ToList();
            }

            return combos;
        }

        public async Task<List<ComboViewModel>> GetCombos()
        {
            return GetCombosDB(false, SessionData.CodLojaSelecionada);
        }

        public async Task<ComboViewModel> GravarCombo(ComboViewModel combo, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    combo.DataHoraInicio = Convert.ToDateTime(combo.DataInicio.Substring(0, 2) + "/" + combo.DataInicio.Substring(3, 2) + "/" + combo.DataInicio.Substring(6) + " " + combo.HoraInicio.Substring(0, 2) + ":" + combo.HoraInicio.Substring(3));
                    combo.DataHoraFim = Convert.ToDateTime(combo.DataFim.Substring(0, 2) + "/" + combo.DataFim.Substring(3, 2) + "/" + combo.DataFim.Substring(6) + " " + combo.HoraFim.Substring(0, 2) + ":" + combo.HoraFim.Substring(3));

                    if (combo.DataHoraFim < combo.DataHoraInicio)
                    {
                        throw new Exception("O término da vigência não pode ser anterior ao início.");
                    }

                    if (modoCadastro == "A") //alteração
                    {
                        var comboAlterar = _contexto.Combos.Find(combo.CodCombo);

                        if (comboAlterar != null)
                        {
                            if (combo.Itens != null)
                            {
                                _contexto.ItensCombo.RemoveRange(_contexto.ItensCombo.Where(c => c.CodCombo == combo.CodCombo.Value).ToList());
                                _contexto.ItensCombo.AddRange(combo.Itens.GroupBy(i => i.CodItemCardapio).Select(o => new ComboItemCardapio { CodCombo = combo.CodCombo.Value, CodItemCardapio = o.First().CodItemCardapio, Quantidade = o.Sum(x => x.Quantidade) }));
                            }

                            if (combo.DiasAssociados != null)
                            {
                                _contexto.DiasSemanaCombo.RemoveRange(_contexto.DiasSemanaCombo.Where(c => c.CodCombo == combo.CodCombo.Value).ToList());
                                _contexto.DiasSemanaCombo.AddRange(combo.DiasAssociados.Select(o => new DiaSemanaCombo { DiaSemana = o.NumDiaSemana, CodCombo = combo.CodCombo.Value }));
                            }

                            comboAlterar.CodEmpresa = combo.CodEmpresa;
                            comboAlterar.NomeCombo = combo.Nome;
                            comboAlterar.DescricaoCombo = combo.Descricao;
                            comboAlterar.Ativo = combo.Ativo;
                            comboAlterar.PrecoCombo = combo.Preco;
                            comboAlterar.DataHoraInicio = combo.DataHoraInicio;
                            comboAlterar.DataHoraFim = combo.DataHoraFim;

                            await _contexto.SaveChangesAsync();
                            dbContextTransaction.Commit();
                        }

                        return combo;
                    }
                    else if (modoCadastro == "I") //inclusão
                    {
                        var comboIncluir = new Combo();
                        if (combo.CodCombo == null)
                        {
                            comboIncluir.CodCombo = 1;
                            var cod = _contexto.Combos.Select(o => o.CodCombo).DefaultIfEmpty(-1).Max();
                            if (cod > 0)
                            {
                                comboIncluir.CodCombo = cod + 1;
                            }
                            combo.CodCombo = comboIncluir.CodCombo;
                        }
                        else
                        {
                            var valida = _contexto.Combos.Find(combo.CodCombo);

                            if (valida != null)
                            {
                                throw new Exception("Já existe um combo cadastrado com o código " + combo.CodCombo);
                            }

                            comboIncluir.CodCombo = combo.CodCombo.Value;
                        }

                        comboIncluir.CodEmpresa = combo.CodEmpresa;
                        comboIncluir.NomeCombo = combo.Nome;
                        comboIncluir.DescricaoCombo = combo.Descricao;
                        comboIncluir.Ativo = combo.Ativo;
                        comboIncluir.PrecoCombo = combo.Preco;
                        comboIncluir.DataHoraInicio = combo.DataHoraInicio;
                        comboIncluir.DataHoraFim = combo.DataHoraFim;

                        _contexto.Combos.Add(comboIncluir);

                        if (combo.Itens != null)
                        {
                            _contexto.ItensCombo.AddRange(combo.Itens.GroupBy(i => i.CodItemCardapio).Select(o => new ComboItemCardapio { CodCombo = combo.CodCombo.Value, CodItemCardapio = o.First().CodItemCardapio, Quantidade = o.Sum(x => x.Quantidade) }));
                        }

                        if (combo.DiasAssociados != null)
                        {
                            _contexto.DiasSemanaCombo.AddRange(combo.DiasAssociados.Select(o => new DiaSemanaCombo { DiaSemana = o.NumDiaSemana, CodCombo = combo.CodCombo.Value }));
                        }

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();

                        return combo;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return null;
        }

        public async Task<string> ExcluiCombo(ComboViewModel combo)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var comboExcluir = await _contexto.Combos.FindAsync(combo.CodCombo);

                    if (comboExcluir != null)
                    {
                        _contexto.ItensCombo.RemoveRange(_contexto.ItensCombo.Where(i => i.CodCombo == combo.CodCombo).ToList());
                        _contexto.DiasSemanaCombo.RemoveRange(_contexto.DiasSemanaCombo.Where(i => i.CodCombo == combo.CodCombo).ToList());

                        _contexto.Combos.Remove(comboExcluir);
                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return "Registro não encontrado na base de dados.";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }


            return "";
        }

        #endregion

        #region Cadastros de promoções de venda

        public async Task<List<TipoAplicacaoDescontoPromocaoViewModel>> GetTiposAplicacaoDesconto()
        {
            return await _contexto.TiposDescontoPromocao
                .OrderBy(d => d.CodTipoAplicacaoDesconto)
                .Select(d => new TipoAplicacaoDescontoPromocaoViewModel
                {
                    CodTipoAplicacaoDesconto = d.CodTipoAplicacaoDesconto,
                    Descricao = d.Descricao
                }).ToListAsync();
        }

        public async Task<List<PromocaoVendaViewModel>> GetPromocoesVenda(int? codEmpresa)
        {
            var programas = await (from promos in _contexto.PromocoesVenda
                                    .Include(p => p.DiasAssociados)
                                    .Include(p => p.Empresa)
                                    .Include(p => p.ClassesAssociadas)
                                    .Include(p => p.ClassesAssociadas.Select(c => c.Classe))
                                    .Include(p => p.ItensAssociados)
                                    .Include(p => p.ItensAssociados.Select(i => i.Item))
                                   join tipo in _contexto.TiposDescontoPromocao on promos.CodTipoAplicacaoDesconto equals tipo.CodTipoAplicacaoDesconto
                                   where promos.PromocaoAtiva && (promos.CodEmpresa != null ? promos.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(promos.CodEmpresa != null ? promos.CodEmpresa.Value : 0))
                                   orderby promos.CodPromocaoVenda
                                   select new PromocaoVendaViewModel
                                   {
                                       CodEmpresa = promos.CodEmpresa,
                                       NomeEmpresa = promos.Empresa.NomeFantasia,
                                       CodPromocaoVenda = promos.CodPromocaoVenda,
                                       DescricaoPromocao = promos.DescricaoPromocao,
                                       DataHoraInicio = promos.DataHoraInicio,
                                       DataHoraFim = promos.DataHoraFim,
                                       CodTipoAplicacaoDesconto = promos.CodTipoAplicacaoDesconto,
                                       DescricaoTipoAplicacaoDesconto = tipo.Descricao,
                                       PercentualDesconto = promos.PercentualDesconto,
                                       PromocaoAtiva = promos.PromocaoAtiva,
                                       DiasAssociados = promos.DiasAssociados.Select(d => new DiaSemanaViewModel
                                       {
                                           NumDiaSemana = d.DiaSemana
                                       }).ToList(),
                                       ClassesAssociadas = promos.ClassesAssociadas.Select(c => new ClasseItemCardapioPromocaoVendaViewModel
                                       {
                                           CodClasse = c.CodClasse,
                                           CodPromocaoVenda = promos.CodPromocaoVenda,
                                           DescricaoClasse = c.Classe.DescricaoClasse
                                       }).ToList(),
                                       ItensAssociados = promos.ItensAssociados.Select(i => new ItemCardapioPromocaoVendaViewModel
                                       {
                                           CodPromocaoVenda = promos.CodPromocaoVenda,
                                           CodItemCardapio = i.CodItemCardapio,
                                           Nome = i.Item.Nome
                                       }).ToList()
                                   }).ToListAsync();

            foreach (var promo in programas)
            {
                promo.DataInicio = promo.DataHoraInicio.ToString("dd/MM/yyyy");
                promo.HoraInicio = promo.DataHoraInicio.ToString("HH:mm");
                promo.DataFim = promo.DataHoraFim.ToString("dd/MM/yyyy");
                promo.HoraFim = promo.DataHoraFim.ToString("HH:mm");
            }

            return programas;
        }

        public async Task<string> ExcluiPromocaoVenda(PromocaoVendaViewModel promo)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var promoExcluir = await _contexto.PromocoesVenda.FindAsync(promo.CodPromocaoVenda);

                    if (promoExcluir != null)
                    {
                        var diasSemanaExcluir = await _contexto.DiasSemanaPromocaoVenda.Where(d => d.CodPromocaoVenda == promo.CodPromocaoVenda).ToListAsync();
                        if (diasSemanaExcluir != null)
                        {
                            _contexto.DiasSemanaPromocaoVenda.RemoveRange(diasSemanaExcluir);
                        }

                        var classesExcluir = await _contexto.ClassesPromocaoVenda.Where(c => c.CodPromocaoVenda == promo.CodPromocaoVenda).ToListAsync();
                        if (classesExcluir != null)
                        {
                            _contexto.ClassesPromocaoVenda.RemoveRange(classesExcluir);
                        }

                        var itensExcluir = await _contexto.ItensPromocaoVenda.Where(i => i.CodPromocaoVenda == promo.CodPromocaoVenda).ToListAsync();
                        if (itensExcluir != null)
                        {
                            _contexto.ItensPromocaoVenda.RemoveRange(itensExcluir);
                        }

                        _contexto.PromocoesVenda.Remove(promoExcluir);
                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return "Registro não encontrado na base de dados.";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }


            return "";
        }


        public async Task<PromocaoVendaViewModel> GravarPromocaoVenda(PromocaoVendaViewModel promocao, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    promocao.DataHoraInicio = Convert.ToDateTime(promocao.DataInicio.Substring(0, 2) + "/" + promocao.DataInicio.Substring(3, 2) + "/" + promocao.DataInicio.Substring(6) + " " + promocao.HoraInicio.Substring(0, 2) + ":" + promocao.HoraInicio.Substring(3));
                    promocao.DataHoraFim = Convert.ToDateTime(promocao.DataFim.Substring(0, 2) + "/" + promocao.DataFim.Substring(3, 2) + "/" + promocao.DataFim.Substring(6) + " " + promocao.HoraFim.Substring(0, 2) + ":" + promocao.HoraFim.Substring(3));

                    if (promocao.DataHoraFim < promocao.DataHoraInicio)
                    {
                        throw new Exception("O término da vigência não pode ser anterior ao início.");
                    }

                    if (modoCadastro == "A") //alteração
                    {
                        var promocaoAlterar = _contexto.PromocoesVenda.Find(promocao.CodPromocaoVenda);

                        if (promocaoAlterar != null)
                        {
                            if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorClasse && promocao.ClassesAssociadas != null)
                            {
                                _contexto.ClassesPromocaoVenda.RemoveRange(_contexto.ClassesPromocaoVenda.Where(c => c.CodPromocaoVenda == promocao.CodPromocaoVenda).ToList());
                                _contexto.ClassesPromocaoVenda.AddRange(promocao.ClassesAssociadas.Select(o => new ClasseItemCardapioPromocaoVenda { CodClasse = o.CodClasse, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                            }
                            else if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorItem && promocao.ItensAssociados != null)
                            {
                                _contexto.ItensPromocaoVenda.RemoveRange(_contexto.ItensPromocaoVenda.Where(i => i.CodPromocaoVenda == promocao.CodPromocaoVenda).ToList());
                                _contexto.ItensPromocaoVenda.AddRange(promocao.ItensAssociados.Select(o => new ItemCardapioPromocaoVenda { CodItemCardapio = o.CodItemCardapio, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                            }

                            if (promocao.DiasAssociados != null)
                            {
                                _contexto.DiasSemanaPromocaoVenda.RemoveRange(_contexto.DiasSemanaPromocaoVenda.Where(d => d.CodPromocaoVenda == promocao.CodPromocaoVenda).ToList());
                                _contexto.DiasSemanaPromocaoVenda.AddRange(promocao.DiasAssociados.Select(o => new DiaSemanaPromocaoVenda { DiaSemana = o.NumDiaSemana, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                            }

                            promocaoAlterar.CodEmpresa = promocao.CodEmpresa;
                            promocaoAlterar.DescricaoPromocao = promocao.DescricaoPromocao;
                            promocaoAlterar.CodTipoAplicacaoDesconto = promocao.CodTipoAplicacaoDesconto;
                            promocaoAlterar.PromocaoAtiva = promocao.PromocaoAtiva;
                            promocaoAlterar.PercentualDesconto = promocao.PercentualDesconto;
                            promocaoAlterar.DataHoraInicio = promocao.DataHoraInicio;
                            promocaoAlterar.DataHoraFim = promocao.DataHoraFim;

                            await _contexto.SaveChangesAsync();
                            dbContextTransaction.Commit();
                        }

                        return promocao;
                    }
                    else if (modoCadastro == "I") //inclusão
                    {
                        var promocaoIncluir = new PromocaoVenda();
                        if (promocao.CodPromocaoVenda <= 0)
                        {
                            promocaoIncluir.CodPromocaoVenda = 1;
                            var cod = _contexto.PromocoesVenda.Select(o => o.CodPromocaoVenda).DefaultIfEmpty(-1).Max();
                            if (cod > 0)
                            {
                                promocaoIncluir.CodPromocaoVenda = cod + 1;
                            }
                            promocao.CodPromocaoVenda = promocaoIncluir.CodPromocaoVenda;
                        }
                        else
                        {
                            var valida = _contexto.PromocoesVenda.Find(promocao.CodPromocaoVenda);

                            if (valida != null)
                            {
                                throw new Exception("Já existe uma promoção cadastrada com o código " + promocao.CodPromocaoVenda);
                            }

                            promocaoIncluir.CodPromocaoVenda = promocao.CodPromocaoVenda;
                        }

                        promocaoIncluir.CodEmpresa = promocao.CodEmpresa;
                        promocaoIncluir.DescricaoPromocao = promocao.DescricaoPromocao;
                        promocaoIncluir.CodTipoAplicacaoDesconto = promocao.CodTipoAplicacaoDesconto;
                        promocaoIncluir.PromocaoAtiva = promocao.PromocaoAtiva;
                        promocaoIncluir.PercentualDesconto = promocao.PercentualDesconto;
                        promocaoIncluir.DataHoraInicio = promocao.DataHoraInicio;
                        promocaoIncluir.DataHoraFim = promocao.DataHoraFim;

                        _contexto.PromocoesVenda.Add(promocaoIncluir);

                        if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorClasse && promocao.ClassesAssociadas != null)
                        {
                            _contexto.ClassesPromocaoVenda.AddRange(promocao.ClassesAssociadas.Select(o => new ClasseItemCardapioPromocaoVenda { CodClasse = o.CodClasse, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                        }
                        else if (promocao.CodTipoAplicacaoDesconto == (int)TipoAplicacaoDescontoEnum.DescontoPorItem && promocao.ItensAssociados != null)
                        {
                            _contexto.ItensPromocaoVenda.AddRange(promocao.ItensAssociados.Select(o => new ItemCardapioPromocaoVenda { CodItemCardapio = o.CodItemCardapio, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                        }

                        if (promocao.DiasAssociados != null)
                        {
                            _contexto.DiasSemanaPromocaoVenda.AddRange(promocao.DiasAssociados.Select(o => new DiaSemanaPromocaoVenda { DiaSemana = o.NumDiaSemana, CodPromocaoVenda = promocao.CodPromocaoVenda }));
                        }

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();

                        return promocao;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return null;
        }


        #endregion

        #region Associação de observações a itens de cardápio

        public async Task GravarObservacoesItens(List<ObservacaoProducaoViewModel> obs, int codItemCardapio, int codClasse)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    if (codClasse > 0)
                    {
                        _contexto.ObservacoesPermitidas.RemoveRange(_contexto.ObservacoesPermitidas.Include(o => o.Item).Where(o => o.Item.CodClasse == codClasse));
                        var itens = _contexto.ItensCardapio.Where(i => i.CodClasse == codClasse).ToList();
                        foreach (var item in itens)
                        {
                            _contexto.ObservacoesPermitidas.AddRange(obs.Select(o => new ObservacaoProducaoPermitidaItemCardapio { CodItemCardapio = item.CodItemCardapio, CodObservacao = o.CodObservacao }));
                        }
                    }

                    if (codItemCardapio > 0)
                    {
                        _contexto.ObservacoesPermitidas.RemoveRange(_contexto.ObservacoesPermitidas.Where(o => o.CodItemCardapio == codItemCardapio));
                        _contexto.ObservacoesPermitidas.AddRange(obs.Select(o => new ObservacaoProducaoPermitidaItemCardapio { CodItemCardapio = codItemCardapio, CodObservacao = o.CodObservacao }));
                    }

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu uma falha durante a execução da operação com a seguinte mensagem: " + ex.Message);
                }
            }
        }

        public async Task GravarOpcoesExtraItens(List<OpcaoExtraViewModel> opcoes, int codItemCardapio, int codClasse)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    if (codClasse > 0)
                    {
                        _contexto.ExtrasPermitidos.RemoveRange(_contexto.ExtrasPermitidos.Include(o => o.Item).Where(o => o.Item.CodClasse == codClasse));
                        var itens = _contexto.ItensCardapio.Where(i => i.CodClasse == codClasse).ToList();
                        foreach (var item in itens)
                        {
                            _contexto.ExtrasPermitidos.AddRange(opcoes.Select(o => new OpcaoExtraItemCardapio { CodItemCardapio = item.CodItemCardapio, CodOpcaoExtra = o.CodOpcaoExtra }));
                        }
                    }

                    if (codItemCardapio > 0)
                    {
                        _contexto.ExtrasPermitidos.RemoveRange(_contexto.ExtrasPermitidos.Where(o => o.CodItemCardapio == codItemCardapio));
                        _contexto.ExtrasPermitidos.AddRange(opcoes.Select(o => new OpcaoExtraItemCardapio { CodItemCardapio = codItemCardapio, CodOpcaoExtra = o.CodOpcaoExtra }));
                    }

                    await _contexto.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu uma falha durante a execução da operação com a seguinte mensagem: " + ex.Message);
                }
            }
        }

        #endregion

        #region Cadastros de opções extra
        public async Task<List<OpcaoExtraViewModel>> GetOpcoesExtra()
        {
            return _contexto.Extras.OrderBy(o => o.CodOpcaoExtra).Select(o => new OpcaoExtraViewModel { CodOpcaoExtra = o.CodOpcaoExtra, DescricaoOpcaoExtra = o.DescricaoOpcaoExtra, Preco = o.Preco }).ToList();
        }

        public async Task<OpcaoExtraViewModel> GravarOpcaoExtra(OpcaoExtraViewModel opcao, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                if (modoCadastro == "A") //alteração
                {
                    var opcaoAlterar = _contexto.Extras.Find(opcao.CodOpcaoExtra);

                    if (opcaoAlterar != null)
                    {
                        opcaoAlterar.DescricaoOpcaoExtra = opcao.DescricaoOpcaoExtra;
                        opcaoAlterar.Preco = opcao.Preco;

                        await _contexto.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }

                    return opcao;
                }
                else if (modoCadastro == "I") //inclusão
                {
                    var opcaoIncluir = new OpcaoExtra();
                    if (opcao.CodOpcaoExtra <= 0)
                    {
                        opcaoIncluir.CodOpcaoExtra = 1;

                        var cod = _contexto.Extras.Select(o => o.CodOpcaoExtra).DefaultIfEmpty(-1).Max();
                        if (cod > 0)
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
                    dbContextTransaction.Commit();

                    return opcao;
                }
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

                    var cod = _contexto.ObservacoesProducao.Select(o => o.CodObservacao).DefaultIfEmpty(-1).Max();
                    if (cod > 0)
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

        #region Parametros de sistema

        public async Task<List<ParametroSistemaViewModel>> GetParametrosSistema(int? codEmpresa)
        {
            return _contexto.ParametrosSistema
                .Include(e => e.Empresa)
                .Where(c => (c.CodEmpresa != null ? c.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(c.CodEmpresa != null ? c.CodEmpresa.Value : 0)))
                .OrderBy(o => o.CodParametro)
                .Select(o => new ParametroSistemaViewModel
                {
                    CodEmpresa = o.CodEmpresa,
                    NomeEmpresa = o.Empresa.NomeFantasia,
                    CodParametro = o.CodParametro,
                    DescricaoParametro = o.DescricaoParametro,
                    ValorParametro = o.ValorParametro
                }).ToList();
        }

        public async Task<ParametroSistemaViewModel> GravarParametroSistema(ParametroSistemaViewModel par, String modoCadastro, int codEmpresa)
        {
            if (modoCadastro == "A") //alteração
            {
                var parAlterar = _contexto.ParametrosSistema.Find(par.CodParametro, codEmpresa);

                if (parAlterar != null)
                {
                    parAlterar.DescricaoParametro = par.DescricaoParametro;
                    parAlterar.ValorParametro = par.ValorParametro;

                    await _contexto.SaveChangesAsync();
                    BrasaoHamburgueria.Web.Helpers.SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.ParametrosSistema);
                }

                return par;
            }
            else if (modoCadastro == "I") //inclusão
            {
                var parIncluir = new ParametroSistema();
                if (par.CodParametro <= 0)
                {
                    parIncluir.CodParametro = 1;

                    var cod = _contexto.ParametrosSistema.Where(p => p.CodEmpresa == codEmpresa).Select(o => o.CodParametro).DefaultIfEmpty(-1).Max();
                    if (cod > 0)
                    {
                        parIncluir.CodParametro = cod + 1;
                    }


                    par.CodParametro = parIncluir.CodParametro;
                }
                else
                {
                    var valida = _contexto.ParametrosSistema.Find(par.CodParametro, codEmpresa);

                    if (valida != null)
                    {
                        throw new Exception("Já existe um parâmetro cadastrado com o código " + par.CodParametro + " na loja " + codEmpresa);
                    }

                    parIncluir.CodParametro = par.CodParametro;
                }

                parIncluir.CodEmpresa = codEmpresa;
                parIncluir.DescricaoParametro = par.DescricaoParametro;
                parIncluir.ValorParametro = par.ValorParametro;

                _contexto.ParametrosSistema.Add(parIncluir);

                await _contexto.SaveChangesAsync();
                BrasaoHamburgueria.Web.Helpers.SessionData.RefreshParam(BrasaoHamburgueria.Web.Helpers.SessionData.ParametrosSistema);

                return par;
            }

            return null;
        }

        public async Task<string> ExcluiParametro(ParametroSistemaViewModel par)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
            {
                try
                {
                    var parExcluir = await _contexto.ParametrosSistema.FindAsync(par.CodParametro, par.CodEmpresa);

                    if (parExcluir != null)
                    {
                        _contexto.ParametrosSistema.Remove(parExcluir);
                        await _contexto.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return "Registro não encontrado na base de dados.";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }


            return "";
        }

        #endregion

        #region Impressoras de produção

        public async Task<List<ImpressoraProducaoViewModel>> GetImpressorasProducao(int? codEmpresa)
        {
            return _contexto.ImpressorasProducao
                .Include(o => o.Empresa)
                .Where(c => (c.CodEmpresa != null ? c.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(c.CodEmpresa != null ? c.CodEmpresa.Value : 0)))
                .OrderBy(o => o.CodImpressora)
                .Select(o => new ImpressoraProducaoViewModel
                {
                    CodEmpresa = o.CodEmpresa,
                    NomeEmpresa = o.Empresa.NomeFantasia,
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
                    impAlterar.CodEmpresa = imp.CodEmpresa;
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

                    var cod = _contexto.ImpressorasProducao.Select(o => o.CodImpressora).DefaultIfEmpty(-1).Max();
                    if (cod > 0)
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

                impIncluir.CodEmpresa = imp.CodEmpresa;
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
        public async Task<List<DadosItemCardapioViewModel>> GetItensCardapioByNome(string chave, int? codEmpresa)
        {
            return _contexto.ItensCardapio
                .Where(i => i.Nome.Contains(chave) && (i.CodEmpresa != null ? i.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(i.CodEmpresa != null ? i.CodEmpresa.Value : 0)))
                .Include(i => i.ObservacoesPermitidas)
                .Include(i => i.ObservacoesPermitidas.Select(o => o.ObservacaoProducao))
                .Include(i => i.ExtrasPermitidos)
                .Include(i => i.ExtrasPermitidos.Select(o => o.OpcaoExtra))
                .OrderBy(i => i.Nome).Take(10)
                .Select(i => new DadosItemCardapioViewModel
                {
                    CodItemCardapio = i.CodItemCardapio,
                    Nome = i.Nome,
                    Observacoes = i.ObservacoesPermitidas.Select(o => new ObservacaoProducaoViewModel { CodObservacao = o.ObservacaoProducao.CodObservacao, DescricaoObservacao = o.ObservacaoProducao.DescricaoObservacao }).ToList(),
                    Extras = i.ExtrasPermitidos.Select(o => new OpcaoExtraViewModel { CodOpcaoExtra = o.OpcaoExtra.CodOpcaoExtra, DescricaoOpcaoExtra = o.OpcaoExtra.DescricaoOpcaoExtra, Preco = o.OpcaoExtra.Preco }).ToList()
                }).ToList();
        }

        public async Task<List<ItemCardapioViewModel>> GetItensCardapio(int? codEmpresa)
        {
            var lista = (
                from itens in _contexto.ItensCardapio
                from classes in _contexto.Classes.Where(c => c.CodClasse == itens.CodClasse)
                from complementos in _contexto.ComplementosItens
                    .Where(c => c.CodItemCardapio == itens.CodItemCardapio).DefaultIfEmpty()
                from imps in _contexto.ImpressorasItens
                    .Where(i => i.CodItemCardapio == itens.CodItemCardapio).Take(1).DefaultIfEmpty()
                from emp in _contexto.Empresas
                    .Where(i => i.CodEmpresa == itens.CodEmpresa).DefaultIfEmpty()
                where (itens.CodEmpresa != null ? itens.CodEmpresa : codEmpresa) == codEmpresa && (SessionData.EmpresasInt.Contains(itens.CodEmpresa != null ? itens.CodEmpresa.Value : 0))
                select new ItemCardapioViewModel
                {
                    CodEmpresa = itens.CodEmpresa,
                    NomeEmpresa = emp.NomeFantasia,
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
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
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
                    dbContextTransaction.Commit();
                }
                else
                {
                    return "Registro não encontrado na base de dados.";
                }
            }

            return "";
        }

        public async Task<ItemCardapioViewModel> GravarItemCardapio(ItemCardapioViewModel item, String modoCadastro)
        {
            using (var dbContextTransaction = _contexto.Database.BeginTransaction())
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

                        itemAlterar.CodEmpresa = item.CodEmpresa;
                        itemAlterar.Nome = item.Nome;
                        itemAlterar.Preco = item.Preco;
                        itemAlterar.Sincronizar = item.Sincronizar;
                        itemAlterar.Ativo = item.Ativo;
                        itemAlterar.CodClasse = item.CodClasse;

                        var complementoAlterar = _contexto.ComplementosItens.Find(item.CodItemCardapio);
                        var semComplemento = false;
                        if (complementoAlterar == null)
                        {
                            complementoAlterar = new ComplementoItemCardapio();
                            complementoAlterar.CodItemCardapio = itemAlterar.CodItemCardapio;
                            semComplemento = true;
                        }

                        if (complementoAlterar != null)
                        {
                            complementoAlterar.DescricaoLonga = item.Complemento.DescricaoLonga;
                            complementoAlterar.Imagem = item.Complemento.Imagem;
                            if (item.Complemento.OrdemExibicao != null)
                            {
                                complementoAlterar.OrdemExibicao = item.Complemento.OrdemExibicao.Value;
                            }

                            if (semComplemento)
                            {
                                _contexto.ComplementosItens.Add(complementoAlterar);
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
                        dbContextTransaction.Commit();
                    }

                    return item;
                }
                else if (modoCadastro == "I") //inclusão
                {
                    var itemIncluir = new ItemCardapio();
                    if (item.CodItemCardapio <= 0)
                    {
                        itemIncluir.CodItemCardapio = 1;

                        var cod = _contexto.ItensCardapio.Select(o => o.CodItemCardapio).DefaultIfEmpty(-1).Max();
                        if (cod > 0)
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

                    itemIncluir.CodEmpresa = item.CodEmpresa;
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
                    dbContextTransaction.Commit();

                    return item;
                }
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
                }).OrderBy(c => c.DescricaoClasse).ToList();

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

                    var cod = _contexto.Classes.Select(o => o.CodClasse).DefaultIfEmpty(-1).Max();
                    if (cod > 0)
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