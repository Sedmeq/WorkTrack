using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model.Dto
{
    public class VacationBalanceDto
    {
        /// <summary>
        /// İşə başladığı tarixdən bu günə qədər hesablanmış cəmi məzuniyyət günləri
        /// </summary>
        public double TotalAccruedDays { get; set; }

        /// <summary>
        /// Təsdiqlənmiş məzuniyyətlərlə istifadə etdiyi günlərin sayı
        /// </summary>
        public double DaysTaken { get; set; }

        /// <summary>
        /// İstifadə edə biləcəyi qalıq məzuniyyət günləri
        /// </summary>
        public double RemainingDays { get; set; }
    }

}
