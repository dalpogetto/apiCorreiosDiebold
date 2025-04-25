using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Text;
using apiCorreios.Data;
using apiCorreios.Models;
using Newtonsoft.Json;

namespace apiCorreios.Services
{
    public class CorreiosService
    {

        public CorreiosService(ApiDbContext ctx)
        {
            this.ctx = ctx;
        }

        //Endpoint
        private readonly string _correiosUrl = "https://api.correios.com.br";

        //Dados da Conta
        private readonly string _usuario = "raianagomes";
        private readonly string _codigoAcesso = "ptqKknMCUIcvTaVk9TKGQxZHSR2XpzAQhlaeXCA1";
        private readonly string _cartaoPostagem = "0070630720";
        private readonly int _dr = 72;

        //Servicos
        private readonly string _SEDEX_12_CONTRATOAG = "03140";
        private readonly string _SEDEX_10_CONTRATOAG = "03158";
        private readonly string _SEDEX_HOJE_CONTRATOAG = "03204";
        private readonly string _SEDEX_CONTRATOAG = "03220";
        private readonly string _PAC_CONTRATOAG = "03298";
        private readonly string _PAC_PC_CONTRATOAG = "04000";
        private readonly string _SEDEX_PC_CONTRATOAG = "04090";
        private readonly ApiDbContext ctx;

        //Informacoes Token
        private static string _contrato;
        private static string _token;
        private static DateTime _expiracaotokenUTC;

        public TokenResponse ObterToken(string usuario, string codigoAcesso, string cartaoPostagem)
        {
            TokenResponse tokenResponse = null;

            

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), _correiosUrl + "/token/v1/autentica/cartaopostagem"))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");

                        var codigo = Convert.ToBase64String(Encoding.UTF8.GetBytes(usuario + ":" + codigoAcesso));

                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + codigo);

                        var tokenRequest = new TokenRequest()
                        {
                            numero = _cartaoPostagem,
                            dr = _dr
                        };

                        var body = JsonConvert.SerializeObject(tokenRequest);

                        request.Content = new StringContent(body);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = httpClient.SendAsync(request).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: seu tratamento de erro aqui
            }

            return tokenResponse;
        }

        public IList<PrecoResponse> ObterPreco(PrecoRequest precoRequest, string token)
        {
            List<PrecoResponse> precoResponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _correiosUrl + "/preco/v1/nacional"))
                    {
                        request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + token);

                        var body = JsonConvert.SerializeObject(precoRequest);

                        var content = new StringContent(body);
                        request.Content = content;
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = client.SendAsync(request).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                            response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            precoResponse = JsonConvert.DeserializeObject<List<PrecoResponse>>(responseBody);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                if (responseBody.Contains("path"))
                                {
                                    var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                                }
                                else
                                {
                                    precoResponse = JsonConvert.DeserializeObject<List<PrecoResponse>>(responseBody);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: seu tratamento de erro aqui
            }

            return precoResponse;
        }

        public IList<PrazoResponse> ObterPrazo(PrazoRequest precoRequest, string token)
        {
            List<PrazoResponse> precoResponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _correiosUrl + "/prazo/v1/nacional"))
                    {
                        request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + token);

                        var body = JsonConvert.SerializeObject(precoRequest);

                        var content = new StringContent(body);
                        request.Content = content;
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = client.SendAsync(request).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                            response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            precoResponse = JsonConvert.DeserializeObject<List<PrazoResponse>>(responseBody);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                if (responseBody.Contains("path"))
                                {
                                    var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                                }
                                else
                                {
                                    precoResponse = JsonConvert.DeserializeObject<List<PrazoResponse>>(responseBody);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: seu tratamento de erro aqui
            }

            return precoResponse;
        }

        public RastroResponse ObterRastreio(string listaObjetosSeparadosPorVirgula)
        {

            //atualiza token e outras infos
            this.AtualizarCorreiosToken();

            RastroResponse objResponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, _correiosUrl + "/srorastro/v1/objetos/" + listaObjetosSeparadosPorVirgula))
                    {
                        request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + _token);

                        var response = client.SendAsync(request).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                            response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            objResponse = JsonConvert.DeserializeObject<RastroResponse>(responseBody);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                if (responseBody.Contains("path"))
                                {
                                    var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                                }
                                else
                                {
                                    objResponse = JsonConvert.DeserializeObject<RastroResponse>(responseBody);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: seu tratamento de erro aqui
            }

            return objResponse;
        }

        public CalculoPrecoPrazoResponse CalcularPrecoPrazo(string cepOrigem, string cepDestino, List<Item> itens)
        {
            var volume = (decimal)0;

            decimal peso = 0;

            foreach (var item in itens)
            {
                var comprimentoItem = (!item.Comprimento.HasValue || item.Comprimento < 16) ? 16 : item.Comprimento.Value;
                var larguraItem = (!item.Largura.HasValue || item.Largura < 11) ? 11 : item.Largura.Value;
                var alturaItem = (!item.Altura.HasValue || item.Altura < 2) ? 2 : item.Altura.Value;

                //Calcula o cm³ volume de cada produto do carrinho.
                var volumeItem = (alturaItem * larguraItem * comprimentoItem);

                //Soma todos os volumes.
                volume += volumeItem;

                //se o peso total for zero, atribui um valor fixo de 200g
                peso += (!item.Peso.HasValue || item.Peso > 0 ? (decimal)(200) : item.Peso.Value);
            }

            //Calcula a raiz cúbica do somatório dos volumes, enviar para os correios o resultado da raíz cúbica no comprimento, largura e volume.
            double potencia = 1.0 / 3.0;
            double raizCubica = Math.Pow(Convert.ToDouble(volume), potencia);

            var comprimento = Convert.ToDecimal(raizCubica);

            var altura = Convert.ToDecimal(raizCubica);

            var largura = Convert.ToDecimal(raizCubica);

            return this.ObterPrecoPrazo(cepOrigem, cepDestino, altura, largura, comprimento, peso);
        }

        private CalculoPrecoPrazoResponse ObterPrecoPrazo(string cepOrigem, string cepDestino, decimal altura, decimal largura, decimal comprimento, decimal peso)
        {

            //atualiza token e outras infos
            this.AtualizarCorreiosToken();

            //pega os códigos dos serviços. Ex: PAC, Sedex etc
            var codigoServicos = BuildCodigoServicosContrato();

            var precoRequest = new PrecoRequest();

            precoRequest.idLote = "1";
            precoRequest.parametrosProduto = new List<PrecoNacionalParam>();

            var prazoRequest = new PrazoRequest();

            prazoRequest.idLote = "1";
            prazoRequest.parametrosPrazo = new List<ParamPrazoNacional>();

            var token = _token;
            var contrato = _contrato;
            var dr = _dr;

            foreach (var codigoServico in codigoServicos)
            {
                var precoNacional = new PrecoNacionalParam
                {
                    cepOrigem = cepOrigem,
                    cepDestino = cepDestino,

                    nuContrato = contrato,
                    nuDR = dr,
                    nuRequisicao = "1",

                    tpObjeto = "2",
                    dtEvento = DateTime.Now.ToString("dd-MM-yyyy"),

                    altura = Math.Round(altura).ToString(),
                    largura = Math.Round(largura).ToString(),
                    diametro = "0",
                    comprimento = Math.Round(comprimento).ToString(),
                    psObjeto = Math.Round(peso).ToString(),
                    coProduto = codigoServico
                };

                var prazoNacional = new ParamPrazoNacional
                {
                    coProduto = codigoServico,
                    cepOrigem = cepOrigem,
                    cepDestino = cepDestino,
                    nuRequisicao = "1",
                    dtEvento = DateTime.Now.ToString("dd-MM-yyyy")
                };

                precoRequest.parametrosProduto.Add(precoNacional);
                prazoRequest.parametrosPrazo.Add(prazoNacional);
            }

            //Chamar api Preco
            var precoResponse = this.ObterPreco(precoRequest, token);

            //Chamar api Prazo
            var prazoResponse = this.ObterPrazo(prazoRequest, token);

            //Concatenar as informacoes
            var opcoesEntrega = this.GerarOpcoesEntrega(precoResponse, prazoResponse);

            return opcoesEntrega;
        }

        private async void AtualizarCorreiosToken()
        {

            var correiosToken = _token;
            var expiracaotokenUTC = _expiracaotokenUTC;


            //Buscar Banco Dados
            var tokenGravado = ctx.cToken.FirstOrDefault();
            if (tokenGravado != null) {
                correiosToken = tokenGravado.token;
                expiracaotokenUTC = tokenGravado.expiraEm;
            }

           
            if (string.IsNullOrEmpty(correiosToken) || CorreiosTokenExpired(expiracaotokenUTC))
            {
                var tokenResponse = this.ObterToken(_usuario, _codigoAcesso, _cartaoPostagem);

                _token = tokenResponse.token;
                _contrato = tokenResponse.cartaoPostagem.contrato;
                _expiracaotokenUTC = tokenResponse.expiraEm;

                //Gravar no Banco
                if (tokenResponse != null) {
                    await ctx.cToken.AddAsync(tokenResponse as CorreiosToken);
                    await ctx.SaveChangesAsync();
                }
            }
        }

        private bool CorreiosTokenExpired(DateTime expiracaotokenUTC)
        {
            bool expired = (expiracaotokenUTC <= DateTime.UtcNow.AddMinutes(-30));

            return expired;
        }

        private IList<string> BuildCodigoServicosContrato()
        {
            var codigos = new List<string>();

            if (!string.IsNullOrEmpty(_PAC_CONTRATOAG))
                codigos.Add(_PAC_CONTRATOAG);

            if (!string.IsNullOrEmpty(_PAC_PC_CONTRATOAG))
                codigos.Add(_PAC_PC_CONTRATOAG);

            if (!string.IsNullOrEmpty(_SEDEX_10_CONTRATOAG))
                codigos.Add(_SEDEX_10_CONTRATOAG);

            if (!string.IsNullOrEmpty(_SEDEX_12_CONTRATOAG))
                codigos.Add(_SEDEX_12_CONTRATOAG);

            if (!string.IsNullOrEmpty(_SEDEX_CONTRATOAG))
                codigos.Add(_SEDEX_CONTRATOAG);
            /*

            if (!string.IsNullOrEmpty(_SEDEX_HOJE_CONTRATOAG))
                codigos.Add(_SEDEX_HOJE_CONTRATOAG);

            if (!string.IsNullOrEmpty(_SEDEX_PC_CONTRATOAG))
                codigos.Add(_SEDEX_PC_CONTRATOAG);
            */

            return codigos;
        }

        private CalculoPrecoPrazoResponse GerarOpcoesEntrega(IList<PrecoResponse> precos, IList<PrazoResponse> prazos)
        {
            var calculoResponse = new CalculoPrecoPrazoResponse();

            foreach (var preco in precos)
            {
                var valor = !string.IsNullOrEmpty(preco.pcFinal) ? decimal.Parse(preco.pcFinal) : 0;

                if (valor > 0)
                {
                   
                    var nome = this.ObterNomeServico(preco.coProduto);
                    var prazo = prazos.FirstOrDefault(c => c.coProduto == preco.coProduto);
                    var prazoEntrega = this.ObterPrazoEntrega(prazo);
                    var msgAdicional = (!string.IsNullOrEmpty(preco.txErro)) ? preco.txErro : string.Empty;
                    var opcaoEntrega = nome + ": R$ " + valor.ToString() + " " + prazoEntrega + (!string.IsNullOrEmpty(preco.txErro) ? ". " + preco.txErro : string.Empty);
                    var objeto = new Calculo { codProduto = preco.coProduto, nomeProduto = nome, valor = valor, prazo = prazoEntrega, mensagem = preco.txErro ?? string.Empty };
                    calculoResponse.calculo.Add(objeto);
                }
                else
                {
                    var nome = this.ObterNomeServico(preco.coProduto);
                    var opcaoEntrega = nome + ": " + preco.txErro;
                    var objeto = new Calculo { codProduto = preco.coProduto, nomeProduto = nome, valor = 0, prazo = string.Empty, mensagem = preco.txErro ?? string.Empty };
                    calculoResponse.calculo.Add(objeto);
                }
            }
            return calculoResponse;
        }

        private string ObterNomeServico(string codigoServico)
        {
            string nomeServico = string.Empty;

            if (codigoServico == this._PAC_CONTRATOAG)
            {
                nomeServico = "PAC";
                return nomeServico;
            }
            else if (codigoServico == this._PAC_PC_CONTRATOAG)
            {
                nomeServico = "PAC_PC";
                return nomeServico;
            }
            else if (codigoServico == this._SEDEX_10_CONTRATOAG)
            {
                nomeServico = "SEDEX_10";
                return nomeServico;
            }
            else if (codigoServico == this._SEDEX_12_CONTRATOAG)
            {
                nomeServico = "SEDEX_12";
                return nomeServico;
            }
            else if (codigoServico == this._SEDEX_CONTRATOAG)
            {
                nomeServico = "SEDEX";
                return nomeServico;
            }
            else if (codigoServico == this._SEDEX_HOJE_CONTRATOAG)
            {
                nomeServico = "SEDEX_HOJE";
                return nomeServico;
            }
            else if (codigoServico == this._SEDEX_PC_CONTRATOAG)
            {
                nomeServico = "SEDEX_PC";
                return nomeServico;
            }


            return nomeServico;
           
        }

        private string ObterPrazoEntrega(PrazoResponse prazo)
        {
            int diasEntrega = 0;

            if (prazo != null && string.IsNullOrEmpty(prazo.txErro))
            {
                diasEntrega = prazo.prazoEntrega;
            }

            if (diasEntrega == 0) return string.Empty;

            var diaUtil = diasEntrega == 1 ? "dia útil" : "dias úteis";

            var prazoEntrega = string.Format("até {0} {1}", diasEntrega, diaUtil);

            return prazoEntrega;
        }
    }
}
    
    