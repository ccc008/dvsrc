package com.kbitubit.tests.dbic;

/** Circular buffer. Remember N last registered messages. */
public class LogRingBuffer {
    /** last N messages, where N = capacity that was passed to the constructor*/
    private final String[] _Buffer;
    /** count of saved messages*/
    private int _Position = 0;
    public LogRingBuffer(int capacity) {
    	_Buffer = new String[capacity];
    }	    
    /** Save message to LogRing */
    public synchronized void append(String message) {
        _Buffer[_Position] = message;
        _Position = (_Position + 1) % _Buffer.length;
    }
    /** Save all messages to single String with \n as delimeter */
    synchronized @Override public String toString() {	    	
    	int size = _Position > _Buffer.length ? _Buffer.length : _Position;
    	return Utils.array2str(_Buffer, size);
    }
}