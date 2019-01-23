package com.example.kanghansung.a2019_01_22;

import android.content.Context;
import android.support.annotation.NonNull;
import android.support.v4.view.PagerAdapter;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.util.ArrayList;

public class pagerAdapter extends PagerAdapter {

    private int[] images = {R.drawable.ic_launcher_foreground, R.drawable.left_p, R.drawable.ic_launcher_background};
    private LayoutInflater inflater;
    private Context context;

    private String[] state = {"유체", "성체"};

    pagerAdapter(Context context){
        this.context = context;
    }

    @Override
    public int getCount() {
        return images.length;
    }

    @NonNull
    @Override
    public Object instantiateItem(@NonNull ViewGroup container, int position) {
//        return super.instantiateItem(container, position);
        inflater = (LayoutInflater)context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        View v = inflater.inflate(R.layout.slider, container, false);

        ImageView imgCharacter = (ImageView)v.findViewById(R.id.imgCharacter);
        TextView tvCharacterName = (TextView)v.findViewById(R.id.tvCharacterName);

        imgCharacter.setImageResource(images[position]);
        tvCharacterName.setText(state[position/2]);

        container.addView(v);

        return v;
    }

    @Override
    public boolean isViewFromObject(@NonNull View view, @NonNull Object o) {
//        return false;
        return view == ((LinearLayout)o);
    }
}
