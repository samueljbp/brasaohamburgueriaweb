import { ItemComandaViewModel, TipoAcaoRegistro } from "./ItemComandaViewModel";

export class ComandaViewModel {
    codEmpresa: number;
    nomeEmpresa: string;
    codComanda: number;
    numMesa: number;
    dataPedido: Date;
    codFormaPagamento: string;
    descricaoFormaPagamento: string;
    trocoPara: number;
    troco: number;
    codBandeiraCartao: number;
    descricaoBandeiraCartao: string;
    valorTotal: number;
    situacao: number;
    descricaoSituacao: string;
    percentualDesconto: number;
    valorDesconto: number;
    motivoDesconto: number;
    className: string;
    itens: ItemComandaViewModel[];

    addItem(item: ItemComandaViewModel) {

        this.itens.push(item);
        item.calculaValorTotalItem();
        this.calculaTotal();

    }

    removeItem(seqItem: number) {

        this.itens.forEach((value, index) => {

            if (value.seqItem == seqItem) {
                this.itens.splice(index, 1);
            }

        });

        this.calculaTotal();

    }

    getItensPedidoMostrar(): ItemComandaViewModel[] {
        let retorno: ItemComandaViewModel[] = new Array<ItemComandaViewModel>();

        this.itens.forEach((value, index) => {

            if (value.acaoRegistro == TipoAcaoRegistro.Incluir || value.acaoRegistro == TipoAcaoRegistro.Alterar) {
                retorno.push(value);
            }

        });

        return retorno;
    }

    getItensComandaMostrar(): ItemComandaViewModel[] {
        let retorno: ItemComandaViewModel[] = new Array<ItemComandaViewModel>();

        this.itens.forEach((value, index) => {

            if (value.acaoRegistro == TipoAcaoRegistro.Incluir || value.acaoRegistro == TipoAcaoRegistro.Alterar || value.acaoRegistro == TipoAcaoRegistro.Nenhuma) {
                retorno.push(value);
            }

        });

        return retorno;
    }

    removeLixo() {
        this.itens.forEach((value, index) => {

            if (value.acaoRegistro == TipoAcaoRegistro.EmInclusao) {
                this.itens.splice(index, 1);
            }

        });

        this.calculaTotal();
    }

    quantidadeItensPendentesGravacao(): number {
        let retorno = 0;

        this.itens.forEach((value, index) => {

            if (value.acaoRegistro == TipoAcaoRegistro.Incluir || value.acaoRegistro == TipoAcaoRegistro.Alterar || value.acaoRegistro == TipoAcaoRegistro.Cancelar) {
                retorno = retorno + 1;
            }

        });

        return retorno;
    }

    getNextSeqItem(): number {
        let retorno: number = 0;

        this.itens.forEach(function (value) {

            if (value.seqItem > retorno) {
                retorno = value.seqItem;
            }

        });

        retorno = retorno + 1;

        return retorno;
    }

    calculaTotal() {

        let total = 0;

        this.itens.forEach(function (value) {

            total = total + value.valorTotal;

        });

        this.valorTotal = total;

    }

    getValorTotalPedido(): number {

        let total = 0;

        this.itens.forEach(function (value) {
            if (value.acaoRegistro == TipoAcaoRegistro.Incluir) {
                total = total + value.valorTotal;
            }

        });

        return total;;

    }

    getValorTotalComanda(): number {

        let total = 0;

        this.itens.forEach(function (value) {
            if (value.acaoRegistro == TipoAcaoRegistro.Incluir ||
                value.acaoRegistro == TipoAcaoRegistro.Alterar ||
                value.acaoRegistro == TipoAcaoRegistro.Nenhuma) {
                total = total + value.valorTotal;
            }

        });

        return total;

    }

    bloqueiaComanda() {
        //coloca comanda em situação de aguardando conta
        this.situacao = 8;
    }

    desbloqueiaComanda() {
        //abre novamente a comanda
        this.situacao = 1;
    }

    comandaAberta(): boolean {
        return (this.situacao == 1);
    }
}