namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

module hljs =
    type [<AllowNullLiteral>] IHighlightResultBase =
        abstract relevance: float with get, set
        abstract language: string with get, set
        abstract value: string with get, set

    and [<AllowNullLiteral>] IAutoHighlightResult =
        inherit IHighlightResultBase
        abstract second_best: IAutoHighlightResult option with get, set

    and [<AllowNullLiteral>] IHighlightResult =
        inherit IHighlightResultBase
        abstract top: ICompiledMode with get, set

    and [<AllowNullLiteral>] HLJSStatic =
        abstract IDENT_RE: string with get, set
        abstract UNDERSCORE_IDENT_RE: string with get, set
        abstract NUMBER_RE: string with get, set
        abstract C_NUMBER_RE: string with get, set
        abstract BINARY_NUMBER_RE: string with get, set
        abstract RE_STARTERS_RE: string with get, set
        abstract BACKSLASH_ESCAPE: IMode with get, set
        abstract APOS_STRING_MODE: IMode with get, set
        abstract QUOTE_STRING_MODE: IMode with get, set
        abstract PHRASAL_WORDS_MODE: IMode with get, set
        abstract C_LINE_COMMENT_MODE: IMode with get, set
        abstract C_BLOCK_COMMENT_MODE: IMode with get, set
        abstract HASH_COMMENT_MODE: IMode with get, set
        abstract NUMBER_MODE: IMode with get, set
        abstract C_NUMBER_MODE: IMode with get, set
        abstract BINARY_NUMBER_MODE: IMode with get, set
        abstract CSS_NUMBER_MODE: IMode with get, set
        abstract REGEX_MODE: IMode with get, set
        abstract TITLE_MODE: IMode with get, set
        abstract UNDERSCORE_TITLE_MODE: IMode with get, set
        abstract ``inherit``: parent: obj * obj: obj -> obj

    and [<AllowNullLiteral>] IModeBase =
        abstract className: string option with get, set
        abstract aliases: ResizeArray<string> option with get, set
        abstract ``begin``: U2<string, Regex> option with get, set
        abstract ``end``: U2<string, Regex> option with get, set
        abstract case_insensitive: bool option with get, set
        abstract beginKeyword: string option with get, set
        abstract endsWithParent: bool option with get, set
        abstract lexems: string option with get, set
        abstract illegal: string option with get, set
        abstract excludeBegin: bool option with get, set
        abstract excludeEnd: bool option with get, set
        abstract returnBegin: bool option with get, set
        abstract returnEnd: bool option with get, set
        abstract starts: string option with get, set
        abstract subLanguage: string option with get, set
        abstract subLanguageMode: string option with get, set
        abstract relevance: float option with get, set
        abstract variants: ResizeArray<IMode> option with get, set

    and [<AllowNullLiteral>] IMode =
        inherit IModeBase
        abstract keywords: obj option with get, set
        abstract contains: ResizeArray<IMode> option with get, set

    and [<AllowNullLiteral>] ICompiledMode =
        inherit IModeBase
        abstract compiled: bool with get, set
        abstract contains: ResizeArray<ICompiledMode> option with get, set
        abstract keywords: obj option with get, set
        abstract terminators: Regex with get, set
        abstract terminator_end: string option with get, set

    and [<AllowNullLiteral>] IOptions =
        abstract classPrefix: string option with get, set
        abstract tabReplace: string option with get, set
        abstract useBR: bool option with get, set
        abstract languages: ResizeArray<string> option with get, set

    type [<Import("*","highlight.js")>] Globals =
        static member IDENT_RE with get(): string = jsNative and set(v: string): unit = jsNative
        static member UNDERSCORE_IDENT_RE with get(): string = jsNative and set(v: string): unit = jsNative
        static member NUMBER_RE with get(): string = jsNative and set(v: string): unit = jsNative
        static member C_NUMBER_RE with get(): string = jsNative and set(v: string): unit = jsNative
        static member BINARY_NUMBER_RE with get(): string = jsNative and set(v: string): unit = jsNative
        static member RE_STARTERS_RE with get(): string = jsNative and set(v: string): unit = jsNative
        static member BACKSLASH_ESCAPE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member APOS_STRING_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member QUOTE_STRING_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member PHRASAL_WORDS_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member C_LINE_COMMENT_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member C_BLOCK_COMMENT_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member HASH_COMMENT_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member NUMBER_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member C_NUMBER_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member BINARY_NUMBER_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member CSS_NUMBER_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member REGEX_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member TITLE_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member UNDERSCORE_TITLE_MODE with get(): IMode = jsNative and set(v: IMode): unit = jsNative
        static member highlight(name: string, value: string, ?ignore_illegals: bool, ?continuation: bool): IHighlightResult = jsNative
        static member highlightAuto(value: string, ?languageSubset: ResizeArray<string>): IAutoHighlightResult = jsNative
        static member fixMarkup(value: string): string = jsNative
        static member highlightBlock(block: Node): unit = jsNative
        static member configure(options: IOptions): unit = jsNative
        static member initHighlighting(): unit = jsNative
        static member initHighlightingOnLoad(): unit = jsNative
        static member registerLanguage(name: string, language: Func<HLJSStatic, IModeBase>): unit = jsNative
        static member listLanguages(): ResizeArray<string> = jsNative
        static member getLanguage(name: string): IMode = jsNative
        static member ``inherit``(parent: obj, obj: obj): obj = jsNative
        static member COMMENT(``begin``: U2<string, Regex>, ``end``: U2<string, Regex>, inherits: IModeBase): IMode = jsNative
