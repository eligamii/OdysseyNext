using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;

namespace Monaco
{
    public enum Language // This enum may be full of typos 
    {
        PlainText,
        Abap,
        Apex,
        Azcli,
        Bat,
        Bicep,
        Camelingo,
        Clojure,
        CoffeeScript,
        C,
        CPP,
        CSharp,
        CSP,
        CSS,
        Cypher,
        Dart,
        Dockerfile,
        Ecl,
        Elixir,
        Flow9,
        FSharp,
        Freemaker2,
        Freemaker2AngleDollar,
        Freemaker2BracketDollar,
        Freemaker2AngleBracket,
        Freemaker2BracketBracket,
        Freemaker2AutoDollar,
        Freemaker2AutoBracket,
        Go,
        Graphql,
        Handlebars,
        Hcl,
        HTML,
        Ini,
        Java,
        JavaScript,
        Julia,
        Kotlin,
        Less,
        Lexon, 
        Lua,
        Liquid,
        M3,
        Markdown,
        Mips,
        Msdax,
        MySQL,
        ObjectiveC,
        Pascal,
        Pascaligo,
        Perl,
        PgSQL,
        Php,
        Pla,
        Postiats,
        Powerquery,
        PowerShell,
        Proto,
        Pug,
        Python,
        QSharp,
        R,
        Razor,
        Redis,
        Redshift,
        RestructuredText,
        Ruby,
        Rust,
        SB,
        Scala,
        Scheme,
        SCSS,
        Shell,
        Sol,
        Aes,
        Sparql,
        SQL,
        ST,
        Swift,
        SystemVerilog,
        Verilog,
        Tcl,
        Twig,
        TypeScript,
        VB,
        XML,
        YAML,
        JSON
    }

    public static class LanguageCoverter
    {
        private static string[] languagesList;
        internal static string LanguageEnumToString(Language enu)
        {
            if(languagesList == null)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguagesList.txt");
                languagesList = File.ReadAllLines(path);
            }

            return languagesList[(int)enu];
        }
    }
}
