import { ItemComanda } from "./ItemComanda";

export class Comanda {
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
    itens: ItemComanda[];

    addItem(item: ItemComanda) {

        this.itens.push(item);
        this.calculaTotal();

    }

    calculaTotal() {

        let total = 0;

        this.itens.forEach(function (value) {

            total = total + value.valorTotalItem;

        });

        this.valorTotal = total;

    }
}