using System.ComponentModel;


namespace Models
{

    public class Product
    {

        [Description("序号")]
        public long Id { get; set; }

        [Description("姓名")]
        public string Name { get; set; }

        [Description("性别")]
        public string Sex { get; set; }

    }

}