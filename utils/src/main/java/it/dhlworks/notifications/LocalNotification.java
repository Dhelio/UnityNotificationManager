package it.dhlworks.notifications;

public class LocalNotification {

    public NotificationData data;
    public int IconId;


    public LocalNotification (String Title, String Description, int IconId) {
        data = new NotificationData();
        this.data.Title = Title;
        this.data.Description = Description;
        this.IconId = IconId;
    }

}
