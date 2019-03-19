using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring
{
    class Zdarzenie
    {
        public DateTime data_godzina_zdarzenia { set; get; }
        public int kamera { set; get; }
        public int zmiana { set; get; }
        public string user { set; get; }
        public DateTime utworzone_data { set; get; }
        public string rodzaj_zdarzenia { set; get; }
        public string przekazanie { set; get; }
        public string lokalizacja { set; get; }
        public Zdarzenie(DateTime data_godzina_zdarzenia, int kamera, int zmiana, string rodzaj_zdarzenia, string przekazanie, string lokalizacja, string user = "", DateTime? utworzone_data = null)
        {
            if (user == "")
            {
                this.user = ActiveUser.User;
                this.utworzone_data = DateTime.Now;
            }
            else
            {
                this.user = user;
                this.utworzone_data = (DateTime)utworzone_data;
            }

            this.data_godzina_zdarzenia = data_godzina_zdarzenia;
            this.kamera = kamera;
            this.zmiana = zmiana;
            this.rodzaj_zdarzenia = rodzaj_zdarzenia;
            this.przekazanie = przekazanie;
            this.lokalizacja = lokalizacja;
        }
    }
}
