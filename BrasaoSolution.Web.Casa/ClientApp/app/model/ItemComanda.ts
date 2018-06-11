import { ObservacaoItemComanda } from "./ObservacaoItemComanda";
import { ExtraItemComanda } from "./ExtraItemComanda";

export class ItemComanda {
    seqItem: number;
    codItem: number;
    descricaoItem: string;
    observacaoLivre: string;
    quantidade: number;
    precoUnitario: number;
    precoUnitarioComDesconto: number;
    valorExtras: number;
    valorTotalItem: number;
    codPromocaoVenda: number;
    percentualDesconto: number;
    valorDesconto: number;
    codCombo: number;
    precoCombo: number;
    descricaoCombo: string;
    acaoRegistro: number;
    obs: ObservacaoItemComanda[];
    extras: ExtraItemComanda[];
}