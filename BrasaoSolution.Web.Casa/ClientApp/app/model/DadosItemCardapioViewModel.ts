import { ObservacaoItemComandaViewModel } from "./ObservacaoItemComandaViewModel";
import { ExtraItemComandaViewModel } from "./ExtraItemComandaViewModel";

export class DadosItemCardapioViewModel {
    codItemCardapio: number;
    nome: string;
    observacoes: ObservacaoItemComandaViewModel[];
    extras: ExtraItemComandaViewModel[];
}