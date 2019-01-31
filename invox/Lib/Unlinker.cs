using System.IO;

namespace invox.Lib {
    /// <summary>
    /// Удаляет XML файлы счета после их архивации
    /// </summary>
    class Unlinker {
        /// <summary>
        /// Удалить файлы счетов
        /// </summary>
        /// <param name="files">Провайдер имен файлов счета</param>
        /// <param name="outputDirectory">Каталог выгрузки</param>
        public static void RemoveFiles(InvoiceFilename files, string outputDirectory) {
#if DEBUG
            string fname = outputDirectory + files.PersonFile + ".xml";
            if (File.Exists(fname)) File.Delete(fname);

            fname = outputDirectory + files.InvoiceFile + ".xml";
            if (File.Exists(fname)) File.Delete(fname);
#endif
        }
    }
}
