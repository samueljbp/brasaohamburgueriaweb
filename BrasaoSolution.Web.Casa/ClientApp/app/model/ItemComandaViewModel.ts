import { ObservacaoItemComandaViewModel } from "./ObservacaoItemComandaViewModel";
import { ExtraItemComandaViewModel } from "./ExtraItemComandaViewModel";

export enum TipoAcaoRegistro {
    Incluir = 0,
    Nenhuma = 1,
    Alterar = 2,
    Cancelar = 3,
    EmInclusao = 4
}

export class ItemComandaViewModel {
    seqItem: number;
    codItem: number;
    descricaoItem: string;
    observacaoLivre: string;
    quantidade: number = 0;
    precoUnitario: number;
    precoUnitarioComDesconto: number;
    valorExtras: number;
    valorTotal: number;
    codPromocaoVenda: number;
    percentualDesconto: number;
    valorDesconto: number;
    codCombo: number;
    precoCombo: number;
    descricaoCombo: string;
    acaoRegistro: number;
    className: string;
    obs: ObservacaoItemComandaViewModel[];
    extras: ExtraItemComandaViewModel[];

    incrementaQuantidade() {
        this.quantidade = this.quantidade + 1;
        this.calculaValorTotalItem();
    }

    decrementaQuantidade() {
        if (this.quantidade > 1) {
            this.quantidade = this.quantidade - 1;
            this.calculaValorTotalItem();
        }
    }

    calculaValorTotalItem() {
        let valor: number = 0.0;
        let valorExtras: number = 0.0;

        if (this.percentualDesconto > 0) {
            valor = valor + this.quantidade * this.precoUnitarioComDesconto;
        } else {
            valor = valor + this.quantidade * this.precoUnitario;
        }

        valorExtras = this.calculaValorExtras();

        this.valorTotal = valor + valorExtras;
    }

    calculaValorExtras(): number {
        let valor: number = 0.0;

        if (this.extras == null) {
            return 0.0;
        }

        this.extras.forEach((value) => {
            valor = valor + this.quantidade * value.preco;
        });

        return valor;
    }

    getExtrasString(): string {
        let retorno: string = "";

        if (this.extras && this.extras.length > 0) {
            this.extras.forEach((value, index) => {
                //retorno = retorno + value.descricaoOpcaoExtra + ' - R$ ' + value.preco.toPrecision(2);
                retorno = retorno + value.descricaoOpcaoExtra + " - " + value.preco.toLocaleString('pt', { style: 'currency', currency: 'BRL' });
                if (index < this.extras.length - 1) {
                    retorno = retorno + ", ";
                }
            });
        }

        return retorno;
    }

    getObsString(): string {
        let retorno: string = "";

        if (this.observacaoLivre && this.observacaoLivre != "") {
            retorno = retorno + this.observacaoLivre;
            if (this.obs && this.obs.length > 0) {
                retorno = retorno + ", ";
            }
        }

        if (this.obs && this.obs.length > 0) {
            this.obs.forEach((value, index) => {
                retorno = retorno + value.descricaoObservacao;
                if (index < this.obs.length - 1) {
                    retorno = retorno + ", ";
                }
            });
        }

        return retorno;
    }

    temExtrasObs(): boolean {
        if (this.getExtrasString() != '' || this.getObsString() != '') {
            return true;
        }

        return false;
    }
}