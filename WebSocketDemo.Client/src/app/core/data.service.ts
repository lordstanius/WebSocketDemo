import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';

@Injectable()
export class DataService {

  socket: WebSocket;
  observer: Observer<number>;

  getQuotes(): Observable<number> {
    this.socket = new WebSocket('ws://localhost:57646/api/connection/1234');
    this.socket.onopen = () => {
      this.socket.onmessage = this.handleMessage;
      this.socket.onerror = this.handleError;
    };

    return new Observable<number>(subscriber => this.observer = subscriber);
  }

  private handleMessage(event: MessageEvent) {
    this.observer.next(event.data);
  }

  private handleError(error) {
    console.error('server error:', error);
    if (error.error instanceof Error) {
      const errMessage = error.error.message;
      return Observable.throw(errMessage);
    }
    return Observable.throw(error || 'Server error');
  }
}
