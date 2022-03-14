using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Namordnik
{
    class PageChange : INotifyPropertyChanged
    {
        public int[] numPage { get; set; } = new int[4]; //отображаемые номера 
        public string[] visible { get; set; } = new string[4]; //видимость текстблоков с номерами
        public string[] bold { get; set; } = new string[4]; //толщина
        public string[] decor { get; set; } = new string[4];//линия внизу

        int countPages;
        int countRowOnPage=20; //количество записей
        int countInList; //Всего записей
        int currentPage;

        public int CountPages //Изменение количества страниц
        {
            get => countPages;
            set
            {
                countPages = value;
                for (int i = 0; i < 4; i++)
                {
                    if (CountPages <= i||numPage[i]>CountPages)
                    {
                        visible[i] = "Hidden";
                    }
                    else
                    {
                        visible[i] = "Visible";
                    }
                }
                OnPropertyChanged("visible");
            }
        }

        public int CountInList
        {
            get => countInList;
            set
            {
                countInList = value;
                if(value%countRowOnPage==0)
                {
                    CountPages = value / countRowOnPage;
                }
                else
                {
                    CountPages = 1 + value / countRowOnPage;
                }
            }
        }

        public int CurrentPage
        {
            get => currentPage;
            set
            {
                int cur = currentPage;
                currentPage = value;
                if(currentPage<1)
                {
                    currentPage = 1;
                }
                if (currentPage > CountPages)
                {
                    currentPage = CountPages;
                }
                else if (CurrentPage > 0)
                {
                    int num1 = numPage[0];
                    int num4 = numPage[3];
                    for (int i = 0; i < 4; i++)
                    {
                        if ((CurrentPage > 3 )&&currentPage>cur) numPage[i]++;
                        if (CurrentPage < num1||num4>=CountPages) numPage[i]--;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (numPage[i] == CurrentPage)
                        {
                            bold[i] = "ExtraBold";
                            decor[i] = "Underline";
                        }
                        else
                        {
                            bold[i] = "Regular";
                            decor[i] = "None";
                        }
                    }
                    CountPages = CountPages;
                }
                OnPropertyChanged("numPage");
                OnPropertyChanged("visible");
                OnPropertyChanged("bold");
                OnPropertyChanged("decor");
            }
        }

        public PageChange(int count)
        {
            for(int i=0; i<4;i++)
            {
                visible[i] = "hidden";
                numPage[i] = i + 1;
                bold[i] = "regular";
            }
            CountInList = count;
            CurrentPage = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
