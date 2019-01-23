package com.example.kanghansung.a2019_01_22;

import android.media.Image;
import android.support.v4.view.ViewPager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity {

    ProgressBar pgbPower;
    ProgressBar pgbDefense;
    ProgressBar pgbHP;
    ViewPager viewpagerCharacter;
    pagerAdapter adapter;

    ImageView imgRight;
    ImageView imgLeft;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        imgLeft = (ImageView)findViewById(R.id.imgLeft);
        imgRight = (ImageView)findViewById(R.id.imgRight);

        pgbDefense = (ProgressBar)findViewById(R.id.pgbDefense);
        pgbHP = (ProgressBar)findViewById(R.id.pgbHP);
        pgbPower = (ProgressBar)findViewById(R.id.pgbPower);

        viewpagerCharacter = (ViewPager)findViewById(R.id.viewpagerCharacter);
        adapter = new pagerAdapter(getApplicationContext());
        viewpagerCharacter.setAdapter(adapter);

        viewpagerCharacter.addOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            @Override
            public void onPageScrolled(int i, float v, int i1) {

        }

            @Override
            public void onPageSelected(int i) {
                switch (i){
                    //hydra
                    case 0:
                        pgbPower.setProgress(35);
                        pgbHP.setProgress(21);
                        pgbDefense.setProgress(18);
                        break;
                    case 1:
                        pgbPower.setProgress(18);
                        pgbHP.setProgress(35);
                        pgbDefense.setProgress(21);
                        break;
                    case 2:
                        pgbPower.setProgress(18);
                        pgbHP.setProgress(18);
                        pgbDefense.setProgress(35);
                        break;
                }
            }

            @Override
            public void onPageScrollStateChanged(int i) {

            }
        });



    }
}
