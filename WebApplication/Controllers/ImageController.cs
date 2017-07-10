using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;


namespace WebApplication.Controllers
{

    /// <summary>
    ///     处理图片，加上水印 原地址：http://www.cnblogs.com/JimmyZheng/archive/2012/04/14/2446507.html
    /// </summary>
    public class ImageController : Controller
    {

        /// <summary>
        ///     处理图片
        /// </summary>
        public void ProcessImage()
        {
            var context = HttpContext;
            context.Response.Clear();

            //得到图片名
            var imageName = context.Request["ImageName"] ?? "1.jpg";

            //得到图片地址
            var stringFilePath = context.Server.MapPath($"~/Images/{imageName}");

            //声明一个FileStream用来将图片暂时放入流中
            var stream = new FileStream(stringFilePath, FileMode.Open);
            using (stream)
            {
                //透过GetImageFromStream方法将图片放入byte数组中
                var imageBytes = GetImageFromStream(stream, context);

                //上下文确定写到客户短时的文件类型
                context.Response.ContentType = "image/jpeg";

                //上下文将imageBytes中的数据写到前段
                context.Response.BinaryWrite(imageBytes);
                stream.Close();
            }

            /*作者原代码
            context.Response.Clear();
            //得到图片名
            var imageName = context.Request["ImageName"] == null ? "逆时针的风"
                : context.Request["ImageName"].ToString();
            //得到图片ID,这里只是演示，实际项目中不是这么做的
            var id = context.Request["Id"] == null ? "01"
                : context.Request["Id"].ToString();
            //得到图片地址
            var stringFilePath = context.Server.MapPath(string.Format("~/Image/{0}{1}.jpg", imageName, id));
            //声明一个FileStream用来将图片暂时放入流中
            FileStream stream = new FileStream(stringFilePath, FileMode.Open);
            using (stream)
            {
                //透过GetImageFromStream方法将图片放入byte数组中
                byte[] imageBytes = this.GetImageFromStream(stream, context);
                //上下文确定写到客户短时的文件类型
                context.Response.ContentType = "image/jpeg";
                //上下文将imageBytes中的数据写到前段
                context.Response.BinaryWrite(imageBytes);
                stream.Close();
            }
            */
        }


        /// <summary>
        ///     将流中的图片信息放入byte数组后返回该数组
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private byte[] GetImageFromStream(FileStream stream, HttpContextBase context)
        {
            //通过stream得到Image
            var image = Image.FromStream(stream);

            //加上水印
            image = SetWaterImage(image, context);

            //得到一个ms对象
            var ms = new MemoryStream();
            using (ms)
            {
                //将图片保存至内存流
                image.Save(ms, ImageFormat.Jpeg);
                var imageBytes = new byte[ms.Length];
                ms.Position = 0;

                //通过内存流读取到imageBytes
                ms.Read(imageBytes, 0, imageBytes.Length);
                ms.Close();

                //返回imageBytes
                return imageBytes;
            }
        }


        /// <summary>
        ///     为图片加上水印，这个方法不用在意，只是演示，所以没加透明度
        ///     下次再加上吧
        /// </summary>
        /// <param name="image">需要加水印的图片</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private Image SetWaterImage(Image image, HttpContextBase context)
        {
            var graphics = Graphics.FromImage(image);
            var waterImage = Image.FromFile(context.Server.MapPath("~/Images/water.jpg"));
           
            //在大图右下角画上水印图就行
            graphics.DrawImage(waterImage,
                new Point
                {
                    X = image.Size.Width - waterImage.Size.Width,
                    Y = image.Size.Height - waterImage.Size.Height
                });
            return image;
        }

    }

}