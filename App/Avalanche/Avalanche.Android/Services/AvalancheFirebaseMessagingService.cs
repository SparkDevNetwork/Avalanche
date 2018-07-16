using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V7.App;
using Android.Util;
using Avalanche.Droid;
using Firebase.Messaging;

namespace FCMClient
{
    [Service]
    [IntentFilter( new[] { "com.google.firebase.MESSAGING_EVENT" } )]
    public class AvalancheFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "AvalancheFirebaseMsgService";
        public override void OnMessageReceived( RemoteMessage message )
        {
            Notification.Builder builder = new Notification.Builder( this )
                     .SetContentText( message.GetNotification().Body )
                     .SetSmallIcon( Resource.Drawable.notification )
                     .SetAutoCancel( true )
                     .SetVisibility( NotificationVisibility.Public );

            var textStyle = new Notification.BigTextStyle();
            textStyle.BigText( message.GetNotification().Body );
            builder.SetStyle( textStyle );

            Intent intent = new Intent( this, typeof( MainActivity ) );
            const int pendingIntentId = 0;
            PendingIntent pendingIntent = PendingIntent.GetActivity( this, pendingIntentId, intent, PendingIntentFlags.OneShot );

            // Launch SecondActivity when the users taps the notification:
            builder.SetContentIntent( pendingIntent );

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService( NotificationService ) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify( notificationId, notification );
        }
    }
}