import { ComplementoItemCardapioViewModel } from "./ComplementoItemCardapioViewModel";
import { OpcaoExtraViewModel } from "./OpcaoExtraViewModel";
import { ObservacaoProducaoViewModel } from "./ObservacaoProducaoViewModel";

export class ItemCardapioViewModel {

    codEmpresa: number;
    nomeEmpresa: string;
    codItemCardapio: number;
    codClasse: number;
    descricaoClasse: string;
    nome: string;
    descricao: string;
    preco: number;
    codPromocaoVenda: number;
    percentualDesconto: number;
    precoComDesconto: number;
    codCombo: number;
    precoCombo: number;
    ativo: boolean;
    sincronizar: boolean;
    codImpressoraProducao: number;
    descricaoImpressoraProducao: string;
    complemento: ComplementoItemCardapioViewModel;
    observacoesPermitidas: ObservacaoProducaoViewModel[];
    extrasPermitidos: OpcaoExtraViewModel[];

}