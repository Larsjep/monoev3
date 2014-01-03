using System;

namespace MonoBrickFirmware.Movement
{
	class PID
	{
		private float k1;
	    private float k2;
	    private float k3;
	   
	    private float Kp;
	    private float Ki;
	    private float Kd;
	   
	    private float Ts;
	   
	    private float ek2;
	    private float ek1;
	   
	    private float uk;
	    private float uk1;
	    private float max;
	    private float min;
	   
	    private float maxChange;
	    private float minChange;
	   
	    private bool maxMinChangeSet;
	    
		private void update(){
	      k1 = Kp;
	      k2 = Kp*(Ts/Ki);
	      k3 = Kp*(Kd/Ts);
	    }
 	 	public PID(float P, float I, float D, float newSampleTime, float maxOut, float minOut, float maxChangePerSec = 0.0f, float minChangePerSec = 0.0f){
			Kp = P;
			Ki = I;
			Kd = D;
			Ts = newSampleTime;
			ek1 = 0;
			ek2 = 0;
			uk1 = 0;
			max = maxOut;
			min = minOut;
			if(maxChangePerSec != 0.0){
			  maxMinChangeSet = true;
			  maxChange = Ts * maxChangePerSec;
			  minChange = Ts * minChangePerSec;
			}
			else{
			  maxMinChangeSet = false;
			}
			update();
		}
		public void setP(float P){
		 	 Kp = P;
		  	update();
		}
		public void setI(float I){
		  	Ki = I;
		  	update();
		}
		public void setD(float D){
		  	Kd = D;
		  	update();
		}
		public void setMaxMin(float newMax, float newMin){
			max = newMax;
			min = newMin;
		}
		
		public void setUk1(float newUk1){
		 	uk1 = newUk1; 
		}
		
		public void setSampleTime(float time){
			Ts = time;
		  	update(); 
		}
		
		public float output(float ek){
			uk = uk1 + k1 *(ek -ek1) + k2*ek +k3*(ek-2*ek1+ek2);
			if(uk > max){uk = max;}
			if(uk < min){uk = min;}
			
			if(maxMinChangeSet){
			  if( (uk-uk1) > maxChange){uk = uk1 + maxChange;}
			  if( (uk-uk1) < minChange){uk = uk1 + minChange;}
			}
			uk1 = uk;
			ek2 = ek1;
			ek1 = ek;
			
			return uk;
		}
	};
}

