package com.unity.mynativeapp;

import android.content.Intent;
import android.os.Bundle;
import android.os.Process;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.FrameLayout;
import com.unity3d.player.UnityPlayerActivity;

import com.company.product.OverrideUnityActivity;

public class MainUnityActivity extends
      //  UnityPlayerActivity{
    OverrideUnityActivity {
    // Setup activity layout
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.i("Unity", "MainUnityActivity::onCreate");
        addControlsToUnityFrame();
        Intent intent = getIntent();
        handleIntent(intent);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        Log.i("Unity", "MainUnityActivity::onNewIntent");
        super.onNewIntent(intent);
        handleIntent(intent);
        setIntent(intent);
    }

    void handleIntent(Intent intent) {
        Log.i("Unity", "MainUnityActivity::intent");
        if(intent == null || intent.getExtras() == null) return;

        if(intent.getExtras().containsKey("doQuit"))
            if(mUnityPlayer != null) {
                Log.i("Unity", "MainUnityActivity::intent doQuit");
                finish();
            }
    }
    @Override
    protected void onStart(){
        super.onStart();
        Log.i("Unity", "MainUnityActivity::onStart");
    }
    @Override
    protected void onRestart(){
        super.onRestart();
        Log.i("Unity", "MainUnityActivity::onRestart");
    }
    @Override
    protected void onResume(){
        super.onResume();
        Log.i("Unity", "MainUnityActivity::onResume");
    }
    @Override
    protected void onPause(){
        super.onPause();
        Log.i("Unity", "MainUnityActivity::onPause");
    }
    @Override
    protected void onStop(){
        super.onStop();
        Log.i("Unity", "MainUnityActivity::onStop");
    }
    @Override
    protected void onDestroy(){
        super.onDestroy();
        Log.i("Unity", "MainUnityActivity::onDestroy");
    }


    @Override
    protected void showMainActivity(String msg) {
        Log.i("Unity","MainUnityActivity::showMainActivity: "+msg);
        Intent intent = new Intent(this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT | Intent.FLAG_ACTIVITY_SINGLE_TOP);
        intent.putExtra("msg", msg);
        startActivity(intent);
    }

    @Override
    public void onUnityPlayerUnloaded() {
        Log.i("Unity","MainUnityActivity::onUnityPlayerUnloaded");
        showMainActivity("");
    }

    public void addControlsToUnityFrame() {
        FrameLayout layout = mUnityPlayer;
        {
            Button myButton = new Button(this);
            myButton.setText("Show Main");
            myButton.setX(10);
            myButton.setY(500);

            myButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                   showMainActivity("");
                }
            });
            layout.addView(myButton, 300, 200);
        }

        {
            Button myButton = new Button(this);
            myButton.setText("Send Msg");
            myButton.setX(320);
            myButton.setY(500);
            myButton.setOnClickListener( new View.OnClickListener() {
                public void onClick(View v) {
                    mUnityPlayer.UnitySendMessage("Ad", "NativeToUnity", "invoke from android");
                }
            });
            layout.addView(myButton, 300, 200);
        }

        {
            Button myButton = new Button(this);
            myButton.setText("Unload");
            myButton.setX(630);
            myButton.setY(500);

            myButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    mUnityPlayer.unload();
                }
            });
            layout.addView(myButton, 300, 200);
        }

        {
            Button myButton = new Button(this);
            myButton.setText("Finish");
            myButton.setX(630);
            myButton.setY(800);

            myButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    finish();
                }
            });
            layout.addView(myButton, 300, 200);
        }
    }


}
