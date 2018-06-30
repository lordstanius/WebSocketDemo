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
      this.socket.onmessage = this.messageHandler.bind(this);
      this.socket.onerror = this.errorHandler.bind(this);
      this.socket.onclose = this.closeHandler.bind(this);
    };

    return new Observable<number>(subscriber => this.observer = subscriber);
  }

  private messageHandler(event: MessageEvent) {
    this.observer.next(event.data);
  }

  private errorHandler() {
    Observable.throw('WebSocket error');
  }

  private closeHandler(event: CloseEvent) {
    Observable.throw(`WebSocket closed: ${event.reason} (${event.code})`);
  }
}
