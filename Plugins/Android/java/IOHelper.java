import android.os.Environment;
import android.os.StatFs;

import java.io.File;

/*
 * IO 操作
 */
public class IOHelper
{
    /*
     * 获取可用磁盘空间 : Get free disk space
     */
    public static long GetFreeDiskSpace()
    {
        try
        {
            File file = Environment.getDataDirectory();
            StatFs sf = new StatFs(file.getPath());
            return sf.getAvailableBytes();
        }
        catch (Throwable e)
        {
            e.printStackTrace();
            return 0;
        }
    }
}