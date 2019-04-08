using Services.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Services;
namespace GPRSSet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ---变量
        AccessHelper accessHelper;
        public ObservableCollection<HuiZhongModel> HuiZhongModels = new ObservableCollection<HuiZhongModel>();
        public ObservableCollection<HuiZhongModel> HuiZhongModelsBranch = new ObservableCollection<HuiZhongModel>();
        public PagingModel<HuiZhongModel> PagingModel;
        private string TreeViewSelectedDisplayName;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IniHuiZhongModels();
           
            //
            List<ZuZhiJiGouModel> ZuZhiModels = new List<ZuZhiJiGouModel>();
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "动力一厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "北区制氧" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "北区球团" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "动力二厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "烧结二厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "新区球团" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "炼钢二厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "炼铁二厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "新区制氧" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "煤气" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "轧钢一厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "轧钢二厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "炼钢一厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "炼铁一厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "水源地" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "白灰厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "烧结一厂" });
            ZuZhiModels.Add(new ZuZhiJiGouModel { Name = "热轧三厂" });
            this.cmbZuZhiJiGou.ItemsSource = ZuZhiModels;
            this.cmbZuZhiJiGou.DisplayMemberPath = "Name";
            this.cmbZuZhiJiGou.SelectedIndex = 0;
            //---
            List<InstrumentTypeModel> ITModels = new List<InstrumentTypeModel>();
            ITModels.Add(new InstrumentTypeModel { Name = "SCL_61D0" });
            ITModels.Add(new InstrumentTypeModel { Name = "积算仪_新" });
            ITModels.Add(new InstrumentTypeModel { Name = "积算仪_AORY5000" });
            ITModels.Add(new InstrumentTypeModel { Name = "积算仪_老" });
            this.cmbInstrumentType.ItemsSource = ITModels;
            this.cmbInstrumentType.DisplayMemberPath = "Name";
            this.cmbInstrumentType.SelectedIndex = 0;
        }

        private void IniHuiZhongModels()
        {
            accessHelper = new AccessHelper();
            DataTable dataTable = accessHelper.GetDataTable("select * from ConnectionSet1");
            if (dataTable == null) return;
            List<HuiZhongModel> lst = dataTable.AsEnumerable().Select(
                m => new HuiZhongModel
                {
                    ID = m.Field<string>("序号"),
                    InstrumentNumber = m.Field<string>("仪表编号"),
                    UserName = m.Field<string>("用户名称"),
                    Organizition = m.Field<string>("组织机构"),
                    SIM = m.Field<string>("SIM卡号"),
                }
            ).ToList();
            HuiZhongModels.Clear();
            lst.ForEach(p => HuiZhongModels.Add(p));

            //记录
            PagingModel = new PagingModel<HuiZhongModel>(HuiZhongModels, 40);
            PagingModel.GetPageData(JumpOperation.GoHome);
            //绑定
            this.txtCurrentPage.SetBinding(TextBlock.TextProperty, new Binding("CurrentIndex") { Source = PagingModel, Mode = BindingMode.TwoWay });
            this.txtTotalPage.SetBinding(TextBlock.TextProperty, new Binding("PageCount") { Source = PagingModel, Mode = BindingMode.TwoWay });
            this.txtTargetPage.SetBinding(TextBox.TextProperty, new Binding("JumpIndex") { Source = PagingModel, Mode = BindingMode.TwoWay });
            this.Record.SetBinding(DataGrid.ItemsSourceProperty, new Binding("ShowDataSource") { Source = PagingModel, Mode = BindingMode.TwoWay });
        }
        private void PageOperationClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            switch (btn.Name)
            {
                case "btnHomePage":
                    PagingModel.GetPageData(JumpOperation.GoHome);
                    break;
                case "btnPreviousPage":
                    PagingModel.GetPageData(JumpOperation.GoPrevious);
                    break;
                case "btnNextPage":
                    PagingModel.GetPageData(JumpOperation.GoNext);
                    break;
                case "btnEndPage":
                    PagingModel.GetPageData(JumpOperation.GoEnd);
                    break;
                case "btnJmpPage":
                    PagingModel.JumpPageData(Convert.ToInt32(txtTargetPage.Text));
                    break;
            }
        }
        private void txtTargetPage_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSim.Text) || string.IsNullOrEmpty(this.txtUserName.Text))
            {
                MessageBox.Show("用户名称或SIM卡号不能为空");
            }
            
            string commandtext = string.Format("CREATE TABLE[dbo].[yg_{0}]("
                + " [记录标示号] int NOT NULL IDENTITY(1,1) ,"
                + " [记录时间] datetime NOT NULL ,"
                + " [更新时间] datetime NULL,"
                + " [记录类型] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL ,"
                + " [记录描述] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL ,"
                + " [电话号] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,"
                + " [瞬时流量] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,"
                + " [累积流量] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,"
                + " [正累积流量] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,"
                + "PRIMARY KEY([记录标示号]))", this.txtSim.Text);
            SqlHelper sql = new SqlHelper();
            sql.Open();
            Console.WriteLine(sql.TestConn);
            if(!sql.Execute(commandtext))
            {
                MessageBox.Show("创建失败！");
                return;
            }
            commandtext = string.Format("select * FROM ConnectionSet1 ORDER BY 序号 DESC");
            DataTable dataTable = accessHelper.GetDataTable(commandtext);
            if (dataTable == null) return;
            List<int> IDS = new List<int>();
            foreach (DataRow dr in dataTable.Rows)
            {
                IDS.Add(int.Parse(dr["序号"].ToString()));
            }
            int ID = IDS.Max();
            ID += 1;
            Console.WriteLine(this.cmbZuZhiJiGou.Text);
            commandtext = string.Format("Insert Into ConnectionSet1" +
                " (序号,组织机构,仪表编号,用户名称,SIM卡号,仪表型号,初装日期)" +
                " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", ID, this.cmbZuZhiJiGou.Text, this.txtSim.Text
                , this.txtUserName.Text, this.txtSim.Text, this.cmbInstrumentType.Text, DateTime.Now);
            if(!accessHelper.Execute(commandtext))
            {
                MessageBox.Show("Access添加失败");
                return;
            }
            MessageBox.Show("创建成功！");
            IniHuiZhongModels();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HuiZhongModel model = (HuiZhongModel)this.Record.SelectedItem;
                if (model != null)
                {
                    Console.WriteLine(model.SIM);

                    string commandtext = string.Format("drop table yg_{0}", model.SIM);
                    SqlHelper sql = new SqlHelper();
                    if(sql.Open())
                    {
                       if(!sql.Execute(commandtext))
                        {
                            MessageBox.Show("删除sql数据库失败！");
                            return;
                        }
                    }
                    commandtext = string.Format("delete from ConnectionSet1 where SIM卡号 = '{0}'", model.SIM);
                    if (!accessHelper.Execute(commandtext))
                    {
                        MessageBox.Show("删除Access数据库失败！");
                        return;
                    }
                    MessageBox.Show("删除成功！");
                    IniHuiZhongModels();
                }
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }
    }
}
