using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushbullet
{
    public interface IPushbulletClient
    {
        bool GetDevices(Action<DeviceList> callback);
        void PushAddress(Device device, string name, string address, Action<PushResponse> callback);
        void PushFile(Device device, string file, Action<PushResponse> callback);
        void PushLink(Device device, string title, string link, Action<PushResponse> callback);
        void PushList(Device device, string title, List<string> items, Action<PushResponse> callback);
        void PushNote(Device device, string title, string body, Action<PushResponse> callback);
    }
}
