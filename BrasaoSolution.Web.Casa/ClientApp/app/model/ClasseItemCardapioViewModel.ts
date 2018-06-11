import { ItemCardapioViewModel } from "./ItemCardapioViewModel";

export class ClasseItemCardapioViewModel {

    codClasse: number;
    descricaoClasse: string;
    sincronizar: boolean;
    codImpressoraPadrao: string;
    descricaoImpressoraPadrao: string;
    imagem: string;
    imagemMini: string;
    ordemExibicao: number;
    itens: ItemCardapioViewModel[];

}