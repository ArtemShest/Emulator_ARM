using Avalonia.Collections;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator_ARM.Models
{
    public class stateBit : ReactiveObject
    {
        private byte _bit1;
        public byte bit1
        {
            get => _bit1;
            set => this.RaiseAndSetIfChanged(ref _bit1, value);
        }
        private byte _bit2;
        public byte bit2
        {
            get => _bit2;
            set => this.RaiseAndSetIfChanged(ref _bit2, value);
        }

        private string _caption;
        public string caption
        {
            get => _caption;
            set => this.RaiseAndSetIfChanged(ref _caption, value);
        }
        public stateBit(string caption)
        {
            this.caption = caption;
        }

    }

    public class PULD : ReactiveObject
    {
        private AvaloniaList<stateBit> _main_status;
        public AvaloniaList<stateBit> main_status
        {
            get => _main_status;
            set => this.RaiseAndSetIfChanged(ref _main_status, value);
        }


        private AvaloniaList<stateBit> _ppu_status;
        public AvaloniaList<stateBit> ppu_status
        {
            get => _ppu_status;
            set => this.RaiseAndSetIfChanged(ref _ppu_status, value);
        }

        public PULD()
        {
            main_status = new()
            {
                new("есть ошибка"),
                new("СУ находится в режиме \"дежурный\""),
                new("СУ находится в режиме \"подготовка\""),
                new("СУ находится в режиме \"рабочий\""),
                new("СУ готова перейтив  следующий режим"),
                new("нет напора в системе охлаждения"),
                new("температура охлаждающей жидкости выше нормы"),
                new("влажность воздуха выше нормы"),
                new("температура воздуха выше нормы"),
                new("температура ЛД ЗГ ревысила 35гр"),
                new("перегрузка ЗГ по току"),
                new("обрыв цепи ЗГ"),
                new("перегрузка ЗГ по напряжению"),
                new("кз в цепях ЗГ"),
                new("температура радиатора ЗГ выше нормы"),
                new("температура радиатора ЗГ ниже нормы"),
                new("нет связи с подсистемами ЗГ"),
                new("сбой микросхемы Пельтье ЗГ"),
                new("перегрев элемента Пельтье ЗГ"),
                new("температура элемента Пельтье ЗГ нестабильна"),
                new("температура печки 1 ЗГ нестабильна"),
                new("температура печки 1 ЗГ нестабильна"),
                new("неисправность нагревателя ТЭНП")
            };
            ppu_status = new()
            {
                new("сбой блока усилителя"),
                new("внешняя блокировка"),
                new("сбой блока питания усилителя"),
                new("ошибка конфигурации сети усилителя"),
                new("ошибка конфигурации блока усилителя"),
                new("ошибка конфигурации канала усилителя"),
                new("перегрев разрядной цепи усилителя"),
                new("превышение импульсного тока канала 0"),
                new("превышение напряжения на транзисторе канала 0"),
                new("перегрев транзистора канала 0"),
                new("канал 0 присуствует"),
                new("превышение импульсного тока канала 1"),
                new("превышение напряжения на транзисторе канала 1"),
                new("перегрев транзистора канала 1"),
                new("канал 1 присуствует"),
                new("превышение импульсного тока канала 2"),
                new("превышение напряжения на транзисторе канала 2"),
                new("перегрев транзистора канала 2"),
                new("канал 2 присуствует"),
                new("превышение импульсного тока канала 3"),
                new("превышение напряжения на транзисторе канала 3"),
                new("перегрев транзистора канала 3"),
                new("канал 3 присуствует"),
                new("превышение импульсного тока канала 4"),
                new("превышение напряжения на транзисторе канала 4"),
                new("перегрев транзистора канала 4"),
                new("канал 4 присуствует"),
                new("превышение импульсного тока канала 5"),
                new("превышение напряжения на транзисторе канала 5"),
                new("перегрев транзистора канала 5"),
                new("канал 5 присуствует"),
                new("нет связи с БП накачки"),
                new(""),
                new("")
            };
        }
    }
}