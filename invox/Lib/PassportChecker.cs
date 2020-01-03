using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Lib {
    /// <summary>
    /// Валидатор номера паспорта
    /// </summary>
    static class PassportChecker {
        // TODO: Форматирование с учетом типа документа F011

        const int SERIAL_LENGTH = 4;
        const int NUMBER_LENGTH = 6;

        static bool SerialGood(string serial) {
            return !string.IsNullOrEmpty(serial)
                && serial.Length == SERIAL_LENGTH
                && serial.All(c => Char.IsDigit(c));
        }

        static bool NumberGood(string number) {
            return !string.IsNullOrEmpty(number)
                && number.Length == NUMBER_LENGTH
                && number.All(c => Char.IsDigit(c));
        }

        /// <summary>
        /// Паспорт пациента соответствует шаблону
        /// </summary>
        /// <param name="person">Пациент</param>
        public static bool Valid(Model.Person person) {
            return person != null && Valid(person.DocumentSerial, person.DocumentNumber);
        }

        /// <summary>
        /// Серия и номер паспорта содержат валидное значение
        /// </summary>
        /// <param name="passport">Серия и номер паспорта. Может содержать пробелы в любом месте</param>
        /// <returns>True, если сведения содержат ровно десять цифр</returns>
        public static bool Valid(string passport) {
            if (string.IsNullOrEmpty(passport))
                return false;

            char[] digits = passport.Where(c => Char.IsDigit(c)).ToArray();

            return digits.Length == SERIAL_LENGTH + NUMBER_LENGTH;
        }

        /// <summary>
        /// Проверка валидности серии и номера паспорта
        /// </summary>
        /// <param name="passportSerial">Серия паспорта</param>
        /// <param name="passportNumber">Номер паспорта</param>
        public static bool Valid(string passportSerial, string passportNumber) {
            return SerialGood(passportSerial) && NumberGood(passportNumber);
        }

        /// <summary>
        /// Установить серию и номер паспорта пациента при наличии различий
        /// </summary>
        /// <param name="person">Пациент, чьи данные требуется сверить</param>
        /// <param name="passportSerial">Серия паспорта</param>
        /// <param name="passportNumber">Номер паспорта</param>
        /// <returns>Истина, если серия и номер паспорта валидные, и удалось обновить
        /// данные пациента при их различии (либо данные пациента бьют)</returns>
        public static bool UpdatePassport(Model.Person person, string passportSerial, string passportNumber) {
            if (person.DocumentType != "14")
                return false;

            if (string.IsNullOrEmpty(passportSerial) && string.IsNullOrEmpty(passportNumber))
                return false;

            char[] digits = (passportSerial + passportNumber).Where(c => Char.IsDigit(c)).ToArray();
            if (digits.Length != SERIAL_LENGTH + NUMBER_LENGTH)
                return false;

            var ser = digits.Take(2).ToList();
            ser.Add(' ');
            ser.AddRange(digits.Skip(2).Take(2));
            passportSerial = new string(ser.ToArray());

            //passportSerial = new string(digits, 0, SERIAL_LENGTH);
            passportNumber = new string(digits, SERIAL_LENGTH, NUMBER_LENGTH);

            if (person.DocumentSerial != passportSerial)
                person.DocumentSerial = passportSerial;

            if (person.DocumentNumber != passportNumber)
                person.DocumentNumber = passportNumber;

            return true;
        }
    }
}
