package it.dhlworks.utils;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.util.Log;

import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import java.util.ArrayList;
import java.util.List;

import it.dhlworks.notifications.LocalNotification;
import it.dhlworks.notifications.NotificationData;

public class Notifications {

    //region CLASSES
    private class NotificationThread extends Thread {

        public volatile boolean Is_Running = false;

        @Override
        public void run() {
            Log.i("NotificationThread", "Thread started");

            Is_Running = true;

            while (localNotificationList.size() > 0) {
                Log.i("NotificationThread", "list size: "+localNotificationList.size());
                try {
                    Thread.sleep(1000 * notificationsInterval);
                } catch (InterruptedException e) {
                    Log.e(TAG,"Fatal error while awaiting!! Aborting thread.");
                    break;
                }

                if (localNotificationList.size() <= 0) //Notifications might've been deleted while the thread was waiting...
                    break;

                LocalNotification tmp = localNotificationList.remove(0);

                Intent appLaunchIntent = new Intent(unityContext.getPackageManager().getLaunchIntentForPackage(targetApk));
                appLaunchIntent.putExtra("NotificationExtra", tmp.data.Title + " " + tmp.data.Description);
                appLaunchIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK | Intent.FLAG_ACTIVITY_NEW_TASK);
                PendingIntent appLaunchPendingIntent = PendingIntent.getActivity(unityContext, 0, appLaunchIntent, PendingIntent.FLAG_UPDATE_CURRENT);

                final NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(unityContext, channelName)
                        .setSmallIcon(tmp.IconId)
                        .setContentTitle(tmp.data.Title)
                        .setContentText(tmp.data.Description)
                        .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                        .setContentIntent(appLaunchPendingIntent)
                        .setAutoCancel(true)
                        .setDefaults(Notification.DEFAULT_SOUND);

                NotificationManagerCompat.from(unityContext).notify(0, notificationBuilder.build());

            }
            Log.i("NotificationThread", "Thread exited.");
            Is_Running = false;
        }

    }
    //endregion

    //region PRIVATE VARIABLE
    private final String TAG = "Utils>Notifications";
    private NotificationThread notificationThread = null;

    private String targetApk;
    private String channelName = null;
    private Context unityContext;
    private List<LocalNotification> localNotificationList;
    private int logoIndex = 0;

    private boolean has_Initialized = false;

    private long notificationsInterval = 60; //Wait time interval in seconds.
    //endregion

    //region PUBLIC FUNCTIONS

    /***
     * Initializes the library
     * @param UnityActivity Calling Unity activity for context.
     */
    public void Initialize(Activity UnityActivity) {
        if (!has_Initialized) {
            has_Initialized = true;

            Log.i(TAG,"Setting activity to open on tap to ["+targetApk+"]");
            targetApk = UnityActivity.getApplicationContext().getPackageName();

            unityContext = UnityActivity;

            localNotificationList = new ArrayList<LocalNotification>();
        }
    }

    /***
     * Sets how much seconds pass between notifications. FOR DEBUG PURPOSES ONLY.
     * @param Interval Time in seconds.
     */
    public void SetNotificationsInterval(int Interval) {
        notificationsInterval = (long) Interval;
    }

    /***
     * Registers, if needed, a notification channel.
     * @param ChannelName The name for the channel
     */
    public void RegisterNotificationChannel(String ChannelName) {
        if (!has_Initialized) {
            Log.e(TAG,"Tried to register notification channel without initializing the library! Call Initialize(Activity) first.");
            return;
        }
        //A notification channel is required for Android's API >26
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            channelName = ChannelName;
            NotificationChannel nChannel = new NotificationChannel(channelName, channelName, NotificationManager.IMPORTANCE_DEFAULT);
            NotificationManager nManager = unityContext.getSystemService(NotificationManager.class);
            nManager.createNotificationChannel(nChannel);
            Log.i(TAG,"Notification channel created.");
        } else {
            Log.w(TAG, "Tried to setup a notification channel, but it is not needed in this android's version.");
        }
    }

    /***
     * Schedules a notification
     * @param Title The title of the notification
     * @param Content The description content of the notification
     */
    public void Schedule(final String Title, final String Content) {

        if (!has_Initialized)
            Log.e(TAG, "Tried to schedule a notification without initializing the library! Call Initialize(Activity) first.");

        int logo;
        switch (logoIndex) {
            case 0:
                logo = R.drawable.logo_1;
                break;
            case 1:
                logo = R.drawable.logo_2;
                break;
            case 2:
                logo = R.drawable.logo_3;
                break;
            case 3:
                logo = R.drawable.logo_4;
                break;
            case 4:
                logo = R.drawable.logo_5;
                break;
            default:
                Log.w(TAG,"Undefined logo index! Setting std logo...");
                logo = R.drawable.logo_1;
                break;
        }
        logoIndex = ((logoIndex + 1) > 4) ? 0 : logoIndex +1;

        localNotificationList.add(new LocalNotification(
                Title,
                Content,
                logo));

        try {
            if (notificationThread == null || !notificationThread.isAlive()) {
                //Creates a thread and runs it
                notificationThread = new NotificationThread();
                notificationThread.start();
            } else {
                Log.w(TAG,"Notification thread already running. Skipping...");
            }
        } catch (IllegalThreadStateException itse) {
            Log.e(TAG,"Tried to run notification thread, but it is already running! Error:\n["+itse+"]\n\nStacktrace: ");
            itse.printStackTrace();
        }
    }

    /***
     * Shows a notification with a placeholder icon.
     * @param Title The title of the notification
     * @param Content The content of the notification
     */
    public void Show(final String Title, final String Content) {
        Intent appLaunchIntent = new Intent(unityContext.getPackageManager().getLaunchIntentForPackage(targetApk));
        appLaunchIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK | Intent.FLAG_ACTIVITY_NEW_TASK);
        PendingIntent appLaunchPendingIntent = PendingIntent.getActivity(unityContext, 0, appLaunchIntent, 0);

        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(unityContext, channelName)
                .setSmallIcon(R.drawable.logo_1)
                .setContentTitle(Title)
                .setContentText(Content)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                .setContentIntent(appLaunchPendingIntent)
                .setAutoCancel(true)
                .setDefaults(Notification.DEFAULT_SOUND);

        Log.d(TAG,"Showing notification NOW.");
        NotificationManagerCompat.from(unityContext).notify(0,notificationBuilder.build());
    }

    /***
     * Cancels all pending notifications
     */
    public void Cancel() {
        localNotificationList.clear();
    }

    /***
     * Cancels a specific notification
     * @param Position The order position of the notification to cancel
     */
    public void Cancel(int Position) {
        if (Position < 0 || Position > localNotificationList.size()) {
            Log.e(TAG,"Cancel error: index out of bounds!");
            return;
        }

        localNotificationList.remove(Position);

    }

    /***
     * Returns an ordered array of active notifications to Unity
     */
    public NotificationData[] GetNotifications() {
        NotificationData[] result = new NotificationData[localNotificationList.size()];
        for (int i=0; i<result.length; i++)
            result[i] = localNotificationList.get(i).data;
        return result;
    }

    /***
     * Reschedules a notification
     * @param CurrentIndex Current order index of the notification to move
     * @param TargetIndex Desired target order index of the notification
     */
    public void Reschedule (int CurrentIndex, int TargetIndex) {
        if (CurrentIndex < 0 || CurrentIndex > localNotificationList.size()) {
            Log.e(TAG,"CurrentIndex out of bounds!");
            return;
        }
        if (TargetIndex < 0 || TargetIndex > localNotificationList.size()) {
            Log.e(TAG, "TargetIndex out of bounds!");
            return;
        }
        LocalNotification tmp = localNotificationList.remove(CurrentIndex);
        localNotificationList.add(TargetIndex, tmp);
    }

    //endregion

}
