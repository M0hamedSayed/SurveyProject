import { Injectable, signal } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { INotification } from '../../common/Interfaces/INotification';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'platform',
})
export class NotificationService {
  private hubConnection!: HubConnection;

  // new notifications
  public notification$ = new BehaviorSubject<INotification | null>(null);

  // connection status
  public connectionStatus = signal<boolean>(false);

  constructor() {
    this.initializeConnection();
    this.registerHandlers();
    this.startConnection();
  }

  ngOnDestroy() {
    this.stopConnection();
  }

  private getAccessToken() {
    return `${localStorage.getItem('accessToken')}`;
  }

  private initializeConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.NOTIFY_API_URL}/notificationHub`, {
        accessTokenFactory: () => this.getAccessToken(),
        skipNegotiation: true, // Important for WebSockets
        transport: HttpTransportType.WebSockets,
        withCredentials: true,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff: 1s, 2s, 4s, 8s, 16s, then max 30s
          return Math.min(
            Math.pow(2, retryContext.previousRetryCount) * 1000,
            30000
          );
        },
      })
      .build();
  }

  private registerHandlers() {
    this.hubConnection.on(
      'ReceiveNotification',
      (message: INotification | null) => {
        if (message) this.notification$.next(message as INotification | null);
        // You could also trigger a UI notification here
      }
    );

    this.hubConnection.onreconnecting(() => {
      this.connectionStatus.set(false);
    });

    this.hubConnection.onreconnected(() => {
      this.connectionStatus.set(true);
    });

    this.hubConnection.onclose((error) => {
      console.log('Connection closed:', error);
      this.connectionStatus.set(false);
    });
  }

  private async startConnection() {
    try {
      await this.hubConnection.start();
      this.connectionStatus.set(true);
    } catch (err) {
      // Implement retry logic or notify user
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  private async stopConnection() {
    if (this.hubConnection) {
      try {
        await this.hubConnection.stop();
      } catch (err) {
        console.error('Error while stopping SignalR connection:', err);
      }
    }
  }
}
