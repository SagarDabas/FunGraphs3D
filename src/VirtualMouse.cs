using System;
using WindowsInput;
using System.Windows.Forms;
namespace FunGraphs3D
{
    

    class VirtualMouse: UtilMPipeline {
        private static MouseSimulator sim;
        private static KeyboardSimulator keySim;
        private int width = Screen.PrimaryScreen.Bounds.Width;
        private int height = Screen.PrimaryScreen.Bounds.Height;
        protected int  nframes;
        protected bool device_lost;

        public VirtualMouse():base() {
            EnableGesture();
            EnableVoiceRecognition();
            nframes=0;
            keySim = new KeyboardSimulator();
            sim = new MouseSimulator();
            device_lost = false;
	    }


        public override void OnGestureSetup(ref PXCMGesture.ProfileInfo pinfo)
        {
            pinfo.alerts = PXCMGesture.Alert.Label.LABEL_FOV_RIGHT | PXCMGesture.Alert.Label.LABEL_FOV_LEFT;
            pinfo.nodeAlerts = PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY | PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_PRIMARY;
            pinfo.sets = PXCMGesture.Gesture.Label.LABEL_POSE_THUMB_UP | PXCMGesture.Gesture.Label.LABEL_NAV_SWIPE_RIGHT;
            pinfo.bodies = PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY | PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_PRIMARY;
        }




        public override void OnGesture(ref PXCMGesture.Gesture data)
        {
            if (data.active)
            {
                if (data.label == PXCMGesture.Gesture.Label.LABEL_POSE_THUMB_UP && data.body == PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY)
                {
                     sim.LeftButtonClick();      
                }
                else if(data.label == PXCMGesture.Gesture.Label.LABEL_NAV_SWIPE_RIGHT)
                {
                    keySim.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
                }
            }

        }


        //public override void OnAlert(ref PXCMGesture.Alert alert)
        //{
        //    if (alert.body == PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY && alert.label == PXCMGesture.Alert.Label.LABEL_FOV_LEFT)
        //    {
        //        secondHand = !secondHand;
        //    }
        //    else if (alert.body == PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_PRIMARY && alert.label == PXCMGesture.Alert.Label.LABEL_FOV_RIGHT)
        //    {
        //        secondHand = false;
        //    }
           
        //}


	    public override bool OnDisconnect()
        {
            if (!device_lost) Console.WriteLine("Device disconnected");
            device_lost = true;
            return base.OnDisconnect();
        }
        public override void OnReconnect()
        {
            Console.WriteLine("Device reconnected");
            device_lost = false;
        }

        public override void OnRecognized(ref PXCMVoiceRecognition.Recognition data)
        {
            //Console.WriteLine("\nRecognized<{0}>", data.dictation);
            if (data.dictation.Equals("Select", StringComparison.OrdinalIgnoreCase))
            {
                sim.LeftButtonClick();
            }
        }


	    public override bool OnNewFrame() 
        {
            
            PXCMGesture gesture = QueryGesture();
            PXCMGesture.GeoNode ndata;
            //if(!secondHand)
            //{
                pxcmStatus sts = gesture.QueryNodeData(0, PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_PRIMARY| PXCMGesture.GeoNode.Label.LABEL_FINGER_THUMB, out ndata);
                if (sts >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    sim.MoveMouseTo(65535 - (65535 * 5 * ndata.positionImage.x) / (double)Screen.PrimaryScreen.Bounds.Width, (65535 * 5 * ndata.positionImage.y) / (double)Screen.PrimaryScreen.Bounds.Height);
                }
             //}
            //else
            //{
            //    pxcmStatus sts = gesture.QueryNodeData(0, PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY |PXCMGesture.GeoNode.Label.LABEL_HAND_MIDDLE, out ndata);
            //    if (sts >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            //    {
            //        if (ndata.opennessState == PXCMGesture.GeoNode.Openness.LABEL_CLOSE && !isPressed)
            //        {
            //            keySim.KeyDown(WindowsInput.Native.VirtualKeyCode.SHIFT);
            //            sim.RightButtonDown();
            //            sim.MoveMouseTo(65535 - (65535 * 5 * ndata.positionImage.x) / (double)Screen.PrimaryScreen.Bounds.Width, (65535 * 5 * ndata.positionImage.y) / (double)Screen.PrimaryScreen.Bounds.Height);
            //            isPressed = true;
            //        }
            //        else if (ndata.opennessState == PXCMGesture.GeoNode.Openness.LABEL_OPEN && isPressed)
            //        {
            //            keySim.KeyUp(WindowsInput.Native.VirtualKeyCode.SHIFT);
            //            sim.RightButtonUp();
            //            isPressed = false;
            //        }
            //    }
            //}

            return (++nframes<50000);
	    }

        
        public void initFingerTracking()
        {
            if (!LoopFrames())
                Console.WriteLine("Failed to initialize or stream data");
            Dispose();
        }
    }
}
