
extern "C" {
  
    long _IOS_GetFreeDiskSpace()
    {
        struct statfs buf;  
        long long freespace = -1;  
        if(statfs("/var", &buf) >= 0){  
            freespace = (long long)(buf.f_bsize * buf.f_bfree);  
        }  
        return freespace/(1024 * 1024);
    }
}
